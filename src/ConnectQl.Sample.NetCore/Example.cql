IMPORT PLUGIN 'Platform'
IMPORT PLUGIN 'Excel'

INSERT INTO FILE('Example.xlsx') SELECT * FROM FILE('Example.json') e
