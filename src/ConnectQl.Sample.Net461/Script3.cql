-- Script3

IMPORT PLUGIN 'Platform'

IMPORT PLUGIN 'Crm'

USE DEFAULT CONNECTIONSTRING('Crm', 'C:\Users\Maarten\Documents\Visual Studio 2017\Projects\ConsoleApp2\ConsoleApp2\App.config') FOR ENTITY

DECLARE @a = 1

SELECT * FROM ENTITY('msdyn_customerasset') e

SELECT s.[Item] +'a' FROM SPLIT('1,2,3,4', ',') s
  
DECLARE @d = SPLIT('1,2,3,4', ',') 

DECLARE @b = '' + 1

DECLARE @c = @b, @a = 2 weeks + 1 second + 3 milliseconds

asdafaa

