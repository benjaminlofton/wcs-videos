[
            {
                "IndexName": "ProviderVideoId-index",
                "Projection": {
                    "ProjectionType": "ALL"
                },
                "ProvisionedThroughput": {
                    "WriteCapacityUnits": 1,
                    "ReadCapacityUnits": 1
                },
                "KeySchema": [
                    {
                        "KeyType": "HASH",
                        "AttributeName": "ProviderVideoId"
                    }
		]
	    }
]

