Feature: API
	As an API User
	I want to interact with the Votus system programmatically
	So that I can automate scenarios specific to me

Scenario: Not Found response is returned when the API cannot find a requested resource
	Given the API User requests a resource that does not exist
	Then the API returns a Not Found response
