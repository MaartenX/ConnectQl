IMPORT PLUGIN 'Platform'
IMPORT PLUGIN 'Excel'

INSERT INTO 
	FILE('Example.xlsx') 
SELECT 
	e.*, 
	s.Item 
FROM 
	FILE('Example.json') e 
		INNER JOIN SPLIT('1,2,3', ',') s 
			ON e.first < s.Item
