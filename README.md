# SharpCredCheck
This C# based executable can quickly verify if obtained credentials are valid in the context of a domain or local machine. 

### Usage:
```
  ___ _                   ___            _  ___ _           _
 / __| |_  __ _ _ _ _ __ / __|_ _ ___ __| |/ __| |_  ___ __| |__
 \__ \ ' \/ _` | '_| '_ \ (__| '_/ -_) _` | (__| ' \/ -_) _| / /
 |___/_||_\__,_|_| | .__/\___|_| \___\__,_|\___|_||_\___\__|_\_\
                   |_|


Required arguments:               Discription:
-------------------               ------------
/user:<username>                  Username of the account you want to verify
/pass:<password>                  Password of the account you want to verify
/local                            Verify credentials on the local machine (use either this one or /ad)
/ad                               Verify credentials in the domain (use either this one or /local)

Optional arguments:               Discription:
-------------------               ------------
/help                             This help menu
/domain:<domain>                  Domain name of the account you want to verify (auto detects current domain name)
/dc:<FQDN or IP>                  FQDN or IP of the domain controller used for authentication (auto detects main DC in current domain)

Usage examples:
---------------
Verify local credentials:         SharpCredCheck.exe /username:john /password:Password123 /local
Verify domain credentials:        SharpCredCheck.exe /username:john /password:Password123 /ad
Verify creds in other domain:     SharpCredCheck.exe /username:john /password:Password123 /domain:contoso.com /dc:dc01.contoso.com /ad
```

### Screenshot:
![Screenshot](https://github.com/pietermiske/SharpCredCheck/blob/main/Screenshots/SharpCredCheck_domain.png?raw=true)