# CodingMonkey.IdentityServer

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/5f14ce52b3094307adcdcca033d7afc4)](https://app.codacy.com/app/thomas.shipley/CodingMonkey.IdentityServer?utm_source=github.com&utm_medium=referral&utm_content=tdshipley/CodingMonkey.IdentityServer&utm_campaign=badger)

An instance of [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) for CodingMonkey

## Required Secrets File

The code requires a two secrets files which are not checked in.

### client.secrets.json

The first is called ```client.secrets.json``` in ```src\CodingMonkey.IdentityServer```. This file contains the clients (the apps / users that can have access to things) which will be defined for use in CodingMonkey.IdentityServer an example of the format and properties is below:

```
[
  {
    "ClientName": "name_of_client",
    "ClientId": "client_id",
    "Enabled": true,
    "AccessTokenType": 1, // This is a enum in IdentityServer4 - refers to the type of token to give to client on auth request
    "ClientSecrets": [
      {
        "Value": "shared_secret_for_client"
      }
    ],
    "AllowedScopes": [
      "list_of_scope_ids_client_can_access"
    ],
    "AllowedGrantTypes": [
      "client_credentials"
    ]
  }
]
```

### scope.secrets.json

The second is called ```scope.secrets.json``` which contains a list of defined scopes (appplications that are protected by identity server) in the system. An example is below:

```
[
  {
    "Name": "scope_name",
    "ScopeSecrets": [
      {
        "Value": "shared_secret_for_scope"
      }
    ]
  }
]
```
