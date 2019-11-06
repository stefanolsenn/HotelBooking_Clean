Feature: Book
	In order to avoid silly mistakes
	As an idiot
	I want to know if I can create a booking


Scenario Outline: Create booking
	Given I have entered a start date <startDate>
	And I have entered an end date <endDate>
	And I have entered a customer ID <customerId>
	When I press the create booking button
	Then The result should be <result>

	Examples:
	  |customerId | startDate    | endDate      |result |
	  |1          | '2020-10-01' | '2020-10-05' |true   |
	  |2          | '2000-10-01' | '2000-10-05' |false  |