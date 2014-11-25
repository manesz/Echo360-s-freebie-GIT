[T4Scaffolding.Scaffolder(Description = "Enter a description of PocoFromDB here")][CmdletBinding()]
param(   
	[parameter(Mandatory=$true, ValueFromPipelinebyPropertyName = $true)][string]$ConnectionString,    
	[parameter(Mandatory=$true, ValueFromPipelinebyPropertyName = $true)][string]$DbContextType, 
	[parameter(Mandatory=$true, ValueFromPipelinebyPropertyName = $true)][string]$TableName, 
	[string]$EntityName,
	[string]$ProviderName, 
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

function createEntity([parameter(Mandatory=$true)] $schema) {
	write-host $schema.Columns.Count
}

function AutoFormatName([parameter(Mandatory=$true)] $tableName) {
	$name = $tableName.ToLower()
	$replacer = @{"_t_" = "_"; "_d_" = "_"}	
	foreach($r in $replacer.GetEnumerator()) {
		$name = $name.Replace($r.Key, $r.Value)
	}
	$pascalNme = ""
	for ($i=0; $i -le $name.Length; $i++) {
		$char = $name[$i]
		if ($i -eq 0) {
			$char = $char.ToString().ToUpper()
		} elseif ($char -eq "_") {
			$i += 1
			if ($i -eq $name.Length) { 
				break
			}
			$char = $name[$i]
			$char = $char.ToString().ToUpper()
		}	
		$pascalName += $char
	}
	return $pascalName
}

#-- Get Entity Name
if ($EntityName.Length -eq 0) {
	$className = AutoFormatName($TableName)
} else {
	$className = $EntityName
}



if (-not $className.ToLower().EndsWith("entity")) {
	$className = $className + "Entity"
}

$classNameWithoutEntity = $className.Replace("Entity", "");

#-- Get Context name
if (-not $DbContextType.ToLower().EndsWith("context")) {
	$DbContextType = $DbContextType + "Context"
}

$currentUser = ([Environment]::UserDomainName + "\" + [Environment]::UserName)
$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
# $provider = "System.Data.SqlClient"
# $provider = "Oracle.DataAccess.Client"
# $provider = "System.Data.SqlServerCe.4.0"


#-- Set SQL as default provider if not specified
if ($providerName.Length -eq 0) {
	$provider = "System.Data.Client"
} else {
	$provider = $providerName 
}

$dbFactory = [System.Data.Common.DbProviderFactories]::GetFactory($provider)
$dbConnection = $dbFactory.CreateConnection()
$dbConnection.ConnectionString = $ConnectionString
$command = $dbConnection.CreateCommand()


if ($TableName.split(" ").Length -eq 1) {
	$command.CommandText = " SELECT * FROM " + $TableName 
} else {
	$command.CommandText = $TableName
}



$schema = New-Object "System.Data.DataTable"
$adapter = $dbFactory.CreateDataAdapter()
$adapter.SelectCommand = $command

try {
	$adapter.FillSchema($schema,[System.Data.SchemaType]::Source)
} catch [system.exception] {
	Write-Host $_.Exception.ToString()  -foregroundcolor red
	Write-Host ("There is an error when connecting to database. Code Generation is cancelled....") -foregroundcolor Magenta
	return
}

# Generate Entity Class
$outputPath = "Models\" + "$className"
Add-ProjectItemViaTemplate $outputPath -Template Entity `
	-Model @{ Namespace = $namespace; `
		ClassName = $className; `
		TableSchema = $schema; `
		User = $currentUser; `
	} `
	-SuccessMessage "Added PocoFromDB output at {0}" `
	-TemplateFolders $TemplateFolders `
	-Project $Project `
	-CodeLanguage $CodeLanguage `
	-Force:$Force 

# Generate Mapping Class
$outputPath = "Mappings\" + "$className" + "Config"
Add-ProjectItemViaTemplate $outputPath -Template Mapping `
	-Model @{ Namespace = $namespace; `
		ClassName = $className; `
		TableSchema = $schema; `
		TableName = $TableName; User = $currentUser; `
	} `
	-SuccessMessage "Added PocoFromDB output at {0}" `
	-TemplateFolders $TemplateFolders `
	-Project $Project `
	-CodeLanguage $CodeLanguage `
	-Force:$Force 
 

# Ensure we can find the model type
$foundModelType = Get-ProjectType $className -Project $Project
if (!$foundModelType) { 
	Write-Host "Did not find type"
	return 
}

$propertyName = Get-PluralizedWord $foundModelType.Name.Replace("Entity", "")

# Find the DbContext class, or create it via a template if not already present
$foundDbContextType = Get-ProjectType ($namespace + "." +$DbContextType) -Project $Project -AllowMultiple
if (!$foundDbContextType) {
	# Write-Host ($namespace + ".Context." +$DbContextType) 
	$outputPath = $DbContextType
	Add-ProjectItemViaTemplate $outputPath -Template Context `
		-Model @{ Namespace = $namespace; `
			ContextName = $DbContextType; `
			User = $currentUser; `
		} `
		-SuccessMessage "Added PocoFromDB output at {0}" `
		-TemplateFolders $TemplateFolders `
		-Project $Project `
		-CodeLanguage $CodeLanguage `
		-Force:$Force 

	$foundDbContextType = Get-ProjectType  ($namespace + "." + $DbContextType) -Project $Project
	if (!$foundDbContextType) { throw "Created database context $DbContextType, but could not find it as a project item" }


}


# Add a new property on the DbContext class
if ($foundDbContextType) {


	Add-ClassMemberViaTemplate -Name $propertyName `
		-CodeClass $foundDbContextType `
		-Template DbContextEntityMember `
		-Model @{ EntityType = $foundModelType; EntityTypeNamePluralized = $propertyName; } `
		-SuccessMessage "Added '$propertyName' to database context '$($foundDbContextType.FullName)'" `
		-TemplateFolders $TemplateFolders `
		-Project $Project `
		-CodeLanguage $CodeLanguage

	$onModel = $foundDbContextType.Members | where {$_.Name -eq "OnModelCreating"}

	if ($onModel -eq $null) {
		$methodName = "OnModelCreating"
		Add-ClassMemberViaTemplate -Name $methodName `
			-CodeClass $foundDbContextType `
			-Template DbContextOnModelCreating `
			-Model @{ ContextType = $foundDbContextType; } `
			-SuccessMessage "Added '$methodName' to database context '$($foundDbContextType.FullName)'" `
			-TemplateFolders $TemplateFolders `
			-Project $Project `
			-CodeLanguage $CodeLanguage
	} else {
		Write-Host ($foundDbContextType.Name + " already has a method called 'OnModelCreating'. Skipping...") -foregroundcolor Magenta
	}
	

	$onModel = $foundDbContextType.Members.Item("OnModelCreating")
	$editPoint = $onModel.StartPoint.CreateEditPoint()
	$entityConfigName = $className + "Config"
	$entityConfig = $editPoint.GetLines($onModel.StartPoint.Line, $onModel.EndPoint.Line+1).split(";") | where {$_.Contains($entityConfigName)}
	
	if ($entityConfig -eq $null) {	
		$editPoint = $onModel.EndPoint.CreateEditPoint()
		$editPoint.StartOfLine()
		$editPoint.LineUp(2)
		$editPoint.EndOfLine()
		$editPoint.InsertNewLine()
		$editPoint.Indent($editPoint,3)
		$editPoint.Insert("modelBuilder.Configurations.Add(new " + $entityConfigName + "());")
		Write-Host ("Added entry for " + $entityConfigName)
	} else {
		Write-Host ("OnModelCreating already has a entry called '" + $entityConfigName + "'. Skipping...") -foregroundcolor Magenta
	}

}

#-- Add Repository pattern
$outputPath = "Repositories\" + "$className" + "Repository"
Add-ProjectItemViaTemplate $outputPath -Template Repository `
		-Model @{ Namespace = $namespace; `
			DbContextType = $foundDbContextType; `
			ModelType = $foundModelType; `
			ModelTypePluralized = $propertyName; `
			TableSchema = $schema; `
			User = $currentUser; `
		} `
		-SuccessMessage "Added PocoFromDB output at {0}" `
		-TemplateFolders $TemplateFolders `
		-Project $Project `
		-CodeLanguage $CodeLanguage `
		-Force:$Force 

# TODO: detect config file


Write-Host "Generation Complete!!!" -foregroundcolor green
Write-Host ([string]::Format("Please remember to add this connectionString on you application config: <add name=`"{0}`" connectionString=`"{1}`" providerName=`"{2}`" />",$DbContextType,$ConnectionString, $provider)) -foregroundcolor green