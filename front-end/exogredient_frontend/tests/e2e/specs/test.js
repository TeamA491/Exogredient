// https://docs.cypress.io/api/introduction/api.html

describe('Search Test', () => {
  it('Search by Ingredient', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(600);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("beef");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Ingredient').click();
    cy.contains('Search').click();
    cy.get('tr').should('have.length',4);
  })

  it("Clicking a store returns correct num of ingredients ", ()=>{
    
    cy.get(".table tr:nth-child(2) td:nth-child(2)").then(($td)=>{
      const ingredientNum = $td.text();
      cy.get(".table tr:nth-child(2) td:nth-child(1) a").click();
      cy.wait(2000);
      cy.get(".table tr").its('length').should('eq',parseInt(ingredientNum)+1);
    })
  })



})
