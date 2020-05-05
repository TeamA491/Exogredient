describe("Test Valid Snapshot Data Tables", () => {
    it("Visit the analysis view and verify the integrity of the data.", () => {
      cy.viewport(1200, 1200) 
      // Arrange
      cy.visit("http://localhost:8080/#/");

      // Act
      cy.contains("Analysis").click();
  
      // Assert
      cy.url().should("include", "/useranalysis");
      cy.contains("td", "david");
      cy.contains("td", "Customer");
      cy.contains("td","United States California")
      cy.contains("td","apple")
      cy.contains("td","walmart")
    });
});

describe("Check if Snapshot Exist.", () => {
    it("Visit the analysis view and verify error message if it does not.", () => {
      cy.viewport(1200, 1200) 
      // Arrange
      cy.visit("http://localhost:8080/#/");

      // Act
      cy.contains("Analysis").click();
  
      // Assert
      cy.url().should("include", "/useranalysis");
      
      cy.get("input[id='monthsTable']").click({force:true});


    });
});