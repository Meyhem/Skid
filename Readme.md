# Skid
Simple, portable CLI utility for configuration templating 

## How it works
Skid will load your JSON files, containing your configuration values, then will render these values into your configuration templates.

## Example
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
Which will render the values from ```local.json``` into ```config.xml.skid``` template creating ```config.xml``` file with filled values
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
Which will render the values from ```prod.json``` into ```config.xml.skid``` template creating ```config.xml``` file with filled values
```xml
<config>
    <connectionString>Server=prodserver.com;Database=ProdDb;User=Tomtheprodadmin;Password=Tom123!</connectionString>
    <logLevel>Warning</logLevel>
</config>
```

## Usage
