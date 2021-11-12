# Skid

Simple, single-file portable CLI utility for configuration templating of all text-based config formats.

### [Download here](https://github.com/Meyhem/Skid/releases/latest)

- [How it works](#how-it-works)
- [Usage](#usage)
- [Example having multiple environments](#example-having-multiple-environments)
- [Features](#features)
  * [Mandatory values](#mandatory-values)
  * [Recursive running](#recursive-running)
  * [Pruning templates](#pruning-templates)
  * [Multiple value files overrides](#multiple-value-files-overrides)
  * [Custom formatting marks](#custom-formatting-marks)
  * [JSON Path selectors](#json-path-selectors)


## How it works

Skid will load your JSON files, containing your configuration values, then it will render these values into your
configuration templates. With single command you can create instantly configured package for specific environment.

## Usage
```
USAGE: skid [--help] --file <path> [--recursive] [--prune] [--mark-start <markStart>] [--mark-end <markEnd>]
            <targetPath>

TARGET:

    <targetPath>          Target skid file or directory to run templating on. In case of directory, it searches all
                          '*.skid' files
OPTIONS:
    --file, -f <path>     Json value file path to load (can be specified multiple times)
    --recursive, -r       Recurse into subdirectories if <target> is directory
    --prune, -p           Delete .skid file template after templating succeeded without error
    --mark-start <markStart> Characters that denote start of value interpolation. Default is '{{'
    --mark-end <markEnd>  Characters that denote end of value interpolation. Default is '}}'
    --help                display this list of options.
```

## Example having multiple environments

Lets assume you have ```config.xml``` file, that you want to have templatable for **local** and **prod** environments

```xml
<config>
    <connectionString>Server=localhost;Database=MyDb;User=Tom;Password=Tom123</connectionString>
    <logLevel>Information</logLevel>
</config>
```

First lets create value file ```local.json``` and fill it with values:

```json
{
  "connectionString": "Server=localhost;Database=MyDb;User=Tom;Password=Tom123",
  "logging": {
    "level": "Information"
  }
}
```

And ```prod.json``` and fill it with values:

```json
{
  "connectionString": "Server=prodserver.com;Database=ProdDb;User=Tomtheprodadmin;Password=Tom123!",
  "logging": {
    "level": "Warning"
  }
}
```

Then create template ```config.xml.skid``` file template from your config

```xml
<config>
    <connectionString>{{!connectionString}}</connectionString>
    <logLevel>{{!logging.level}}</logLevel>
</config>
```

---
**Then run skid for local environment**

```sh
skid -f local.json config.xml.skid
```

Which will render the values from ```local.json``` into ```config.xml.skid``` template creating ```config.xml``` file
with filled values

```xml
<config>
    <connectionString>Server=localhost;Database=MyDb;User=Tom;Password=Tom123</connectionString>
    <logLevel>Information</logLevel>
</config>
```

---
**Or run skid for prod environment**

```sh
skid -f prod.json config.xml.skid
```

Which will render the values from ```prod.json``` into ```config.xml.skid``` template creating ```config.xml``` file
with filled values

```xml
<config>
    <connectionString>Server=prodserver.com;Database=ProdDb;User=Tomtheprodadmin;Password=Tom123!</connectionString>
    <logLevel>Warning</logLevel>
</config>
```

## Features

### Mandatory values

In case of many config values in multiple files, it's easy to miss something. Skid allows you to mark mandatory (
non-empty) in your templates which will generate warning if the value for interpolation is missing.  
Simply add "!" in front of json selector to mark interpolation as mandatory like this:

```sh
connectionString={{ ! connectionString }}
```

will produce warning

```
<!> missing required value 'connectionString ' used in 'D:\app\config.txt.skid'
```

### Recursive running

Skid supports specifying the target either by File or Folder (in case of folder Skid searches for _.skid_ files).

```sh
skid -f values.json config.xml.skid
skid -f values.json my-folder-with-configs
```

You can specify the ```-r``` option to recursively scan target folder and its children for _.skid_ files.

```sh
skid -r -f values.json my-app-package-with-deep-structure
```

### Pruning templates
Skid allows you to pass ```--prune``` or ```-p``` option, which deletes .skid file templates
after it's rendered without any error.  
This feature is handy if you want to clear your deployment package of .skid templates after 
they are used and are no longer needed.
```sh
skid -f values.json --prune config.xml.skid
skid -f values.json -p config.xml.skid
```

### Multiple value files overrides

Skid supports loading multiple JSON files and merge their values. This feature is handy for example when
having ```base.json``` filled with commom values, then ```prod.json``` containing production specific values
and ```secrets.json``` containing access credentials that is kept outside version control.  
For such case you can create these JSON files and feed them to skid like this:

```sh
skid -f base.json -f prod.json -f secrets.json ./MyApplicationPackage
```

Note that **-f** options can be specified multiple times and are order dependent, the **last available value wins**.

### Custom formatting marks

Skid by default uses the '{{ ... }}' style interpolation. In some rare scenarios these might be already used for
different purposes. Skid allows you to define custom formatting marks to avoid this scenario simply like this:

```
skid -f values.json --mark-start "<<" --mark-end ">>" app-package
```

Skid will now look for value interpolations '<< ... >>'

### JSON Path selectors

Skid supports [JsonPath](https://goessner.net/articles/JsonPath/index.html#e2) selectors in your template interpolations
allowing you to select deep nested properties through arrays and maps.   
Example values file:

```json
{
  "Tenants": [
    {
      "Region": {
        "Name": "Europe"
      }
    }
  ]
}
```
The value can be interpolated via ```{{ Tenants[0].Region.Name }}```  
See [JsonPath docs](https://goessner.net/articles/JsonPath/index.html#e2) for all features.
