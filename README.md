# Cool-Project
Service that fetch currency rates using data from the National Bank of Ukraine (NBU) API

Custom Control Codes(from command line):
1. "coolproject.exe -install"
   
   This command installs the service as an administrator
   
3. "coolproject.exe -uninstall"
   This command uninstalls the service as an administrator
   
4. "coolproject.exe -start"
   This command starts the service as administrator
   
5. "coolproject.exe -stop"
   This command stops the service as administrator
   
6. "coolproject.exe -fmt xml"
   This command runs changes the file type, in which format and file the exchange rates will be stored
   
7. "coolproject.exe -out c:\download"
   This command changes the path where the file will be stored
   
8. "olproject.exe -interval 10"
   This command changes the interval at which the service will receive data on exchange rates. The value is the number of seconds
   
9. "coolproject.exe -currency usd"
   This command sets the currency for which you want to get the exchange rate
   
P.S. You can combine teams. Example: "tool project.the -interval 10 -fmt hml"

Config File:

[Settings]

fileType = csv                                                                           -  in which format and file type the exchange rates will be stored

execInterval = 5                                                                         -  interval at which the service will receive data on exchange rates(sec). 

filePath = D:\Projects\abz.agency\CoolProject\CoolProject\bin\Debug\Downloads\           -  the path where the file will be stored

[RequestData]

urlNBUByPeriod = https://bank.gov.ua/NBU_Exchange/exchange_site                          - url to get exchange rates by period

urlNBUByDate = https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange              - url to get exchange rates by date

startDate = 01.01.2024                                                                   - period start date. Format only dd.MM.yyyy

endDate = 22.07.2024                                                                     - period end date. Format only dd.MM.yyyy

valcode = usd                                                                            - еhe currency in which you need to get the rate


