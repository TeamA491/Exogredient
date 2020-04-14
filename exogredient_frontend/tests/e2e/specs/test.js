describe('General Test', ()=>{

  it('Clicking "exogredient" resets to home page', ()=>{
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("beef");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Ingredient').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('a').contains('exogredient').click();
    cy.url().should('eq', 'http://localhost:8080/');
    cy.get('input[name="street"]').should('be.empty');
    cy.get('input[name="search"]').should('be.empty');
    cy.get('input[name="mile"]').should('be.empty');
    cy.get('span strong:first').contains('ingredient');
  });

  it('Display error page when backend sends error status code', ()=>{
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("error");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Ingredient').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.url().should('include', '/ErrorView');
    cy.get('input[name="street"]').should('be.empty');
    cy.get('input[name="search"]').should('be.empty');
    cy.get('input[name="mile"]').should('be.empty');
    cy.get('span strong:first').contains('ingredient');
    cy.contains('Something went wrong... Please try again later.');
  });

})

describe('SearchResultsView Test', () => {

  it('Search by existing ingredient gives results', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("beef");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Ingredient').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.url().should('include', '/SearchResultsView');
    cy.get('tr').its('length').should('eq',4);
  })

  it('Search by non-existing ingredient gives no results', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("asdasd");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Ingredient').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.contains('No Results');
      
  })

  it('Search by existing store gives results', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.url().should('include', '/SearchResultsView');
    cy.get('tr').its('length').should('eq',21);
  })

  it('Search by non-existing store gives no results', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("asdasd");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.contains('No Results');
  })

  it('Search with non-autocompleted address gives a warning alert', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.get('input[name="search"]').type("beef");
    cy.get('input[name="mile"]').type(20);
    cy.contains('Ingredient').click();
    const stub = cy.stub();
    cy.on('window:alert', stub)
    cy.contains('Search').click().then(()=>{
      expect(stub.getCall(0)).to.be.calledWith("Address must be selected from the autocompletes");
    });
    cy.url().should('eq', 'http://localhost:8080/');
  })

  it('Search with empty search-term gives a warning alert', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="mile"]').type(20);
    const stub = cy.stub();
    cy.on('window:alert', stub)
    cy.contains('Search').click().then(()=>{
      expect(stub.getCall(0)).to.be.calledWith("The search term cannot be empty");
    });
    cy.url().should('eq', 'http://localhost:8080/');
  })

  it('Search with radius out of range gives a warning alert', () => {
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(0);
    cy.contains('Store').click();
    const stub = cy.stub();
    cy.on('window:alert', stub)
    cy.contains('Search').click().then(()=>{
      expect(stub.getCall(0)).to.be.calledWith("Radius must be a number between 1-100");
    });
    cy.url().should('eq', 'http://localhost:8080/');
  })

  it("Successfully move page by clicking page number", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('a[class="pagination-link is-current"').should('have.length',1);
    cy.get('a[class="pagination-link is-current"').contains('1');
    cy.get('.table tr').its('length').should('eq', 21);
    cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td)=>{
      const storename1 = $td.text();
      cy.get('a[class="pagination-link"]').contains("2").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('2');
      cy.get('.table tr').its('length').should('eq', 2);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename1).to.not.equal(storename2);
      });
      cy.get('a[class="pagination-link"]').contains("1").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('1');
      cy.get('.table tr').its('length').should('eq', 21);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename2).to.equal(storename1);
      });
    });
  })

  it("Successfully move page with Next and Previous button", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('a[class="pagination-link is-current"').should('have.length',1);
    cy.get('a[class="pagination-link is-current"').contains('1');
    cy.get('.table tr').its('length').should('eq', 21);
    cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td)=>{
      const storename1 = $td.text();
      cy.get('button').contains("Next").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('2');
      cy.get('.table tr').its('length').should('eq', 2);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename1).to.not.equal(storename2);
      });
      cy.get('button').contains("Previous").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('1');
      cy.get('.table tr').its('length').should('eq', 21);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename2).to.equal(storename1);
      });
    });
  })

  it("Disabled Previous Button & Actice Next Button at page 1", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('button').contains('Previous').should('be.disabled');
    cy.get('button').contains('Next').should('not.be.disabled');
  })

  it("Active Previous Button & Disabled Next Button at last page", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('a[class="pagination-link"]').contains("2").click();
    cy.get('button').contains('Next').should('be.disabled');
    cy.get('button').contains('Previous').should('not.be.disabled');
  })

  it('Sort by distance', ()=>{
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('strong').contains('distance (ascending)');
    cy.get(".table tr:nth-child(2) td:nth-child(3)").then(($td)=>{
      const minDistance = $td.text();
      cy.get('a').contains('Distance').click();
      cy.wait(300);
      cy.get('strong').contains('distance (descending)');
      cy.get(".table tr:nth-child(2) td:nth-child(3)").then(($td2)=>{
        const maxDistance = $td2.text();
        cy.get('a[class="pagination-link"]').contains("2").click();
        cy.get(".table tr:nth-child(2) td:nth-child(3)").then(($td3)=>{
          expect(minDistance).to.equal($td3.text());
        })
        cy.get('a').contains('Distance').click();
        cy.wait(300);
        cy.get('strong').contains('distance (ascending)');
        cy.get('a[class="pagination-link"]').contains("2").click();
        cy.get(".table tr:nth-child(2) td:nth-child(3)").then(($td3)=>{
          expect(maxDistance).to.equal($td3.text());
        })
      })
    })
  })

  it('Sort by ingredients num', ()=>{
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get('a').contains('Number of ingredients').click();
    cy.wait(300);
    cy.get('strong').contains('number of ingredients (descending)');
    cy.get(".table tr:nth-child(2) td:nth-child(2)").then(($td)=>{
      const maxNum = $td.text();
      cy.get('a').contains('Number of ingredients').click();
      cy.wait(300);
      cy.get('strong').contains('number of ingredients (ascending)');
      cy.get(".table tr:nth-child(2) td:nth-child(2)").then(($td2)=>{
        const minNum = $td2.text();
        cy.get('a[class="pagination-link"]').contains("2").click();
        cy.wait(300);
        cy.get(".table tr:nth-child(2) td:nth-child(2)").then(($td3)=>{
          expect(maxNum).to.equal($td3.text());
        })
        cy.get('a').contains('Number of ingredients').click();
        cy.wait(300);
        cy.get('strong').contains('number of ingredients (descending)');
        cy.get('a[class="pagination-link"]').contains("2").click();
        cy.wait(300);
        cy.get(".table tr:nth-child(2) td:nth-child(2)").then(($td3)=>{
          expect(minNum).to.equal($td3.text());
        })
      })
    })
  })

})



describe('StoreView Test', ()=>{
  
  it("Clicking a store returns correct num of ingredients ", ()=>{
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("beef");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Ingredient').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get(".table tr:nth-child(2) td:nth-child(2)").then(($td)=>{
      const ingredientNum = $td.text();
      cy.get(".table tr:nth-child(2) td:nth-child(1) a").click();
      cy.wait(300);
      cy.url().should('include', '/StoreView');
      cy.get(".table tr").its('length').should('eq',parseInt(ingredientNum)+1);
    })
  })

  it('Successfully move page by clicking page number',()=>{
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(100);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get(".table tr:nth-child(5) td:nth-child(1) a").click();
    cy.wait(300);
    cy.get('a[class="pagination-link is-current"').should('have.length',1);
    cy.get('a[class="pagination-link is-current"').contains('1');
    cy.get('.table tr').its('length').should('eq', 21);
    cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td)=>{
      const storename1 = $td.text();
      cy.get('a[class="pagination-link"]').contains("2").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('2');
      cy.get('.table tr').its('length').should('eq', 17);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename1).to.not.equal(storename2);
      });
      cy.get('a[class="pagination-link"]').contains("1").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('1');
      cy.get('.table tr').its('length').should('eq', 21);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename2).to.equal(storename1);
      });
    });
  })

  it("Successfully move page with Next and Previous button", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get(".table tr:nth-child(5) td:nth-child(1) a").click();
    cy.wait(300);
    cy.get('a[class="pagination-link is-current"').should('have.length',1);
    cy.get('a[class="pagination-link is-current"').contains('1');
    cy.get('.table tr').its('length').should('eq', 21);
    cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td)=>{
      const storename1 = $td.text();
      cy.get('button').contains("Next").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('2');
      cy.get('.table tr').its('length').should('eq', 17);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename1).to.not.equal(storename2);
      });
      cy.get('button').contains("Previous").click();
      cy.wait(300);
      cy.get('a[class="pagination-link is-current"').should('have.length',1);
      cy.get('a[class="pagination-link is-current"').contains('1');
      cy.get('.table tr').its('length').should('eq', 21);
      cy.get(".table tr:nth-child(2) td:nth-child(1)").then(($td2)=>{
        const storename2 = $td2.text();
        expect(storename2).to.equal(storename1);
      });
    });
  })

  it("Disabled Previous Button & Actice Next Button at page 1", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get(".table tr:nth-child(5) td:nth-child(1) a").click();
    cy.wait(300);
    cy.get('button').contains('Previous').should('be.disabled');
    cy.get('button').contains('Next').should('not.be.disabled');
  })

  it("Active Previous Button & Disabled Next Button at last page", ()=>{
    var chai = require('chai');
    var expect = chai.expect;
    cy.visit("http://localhost:8080/")
    cy.get('input[name="street"]').type("1250");
    cy.wait(700);
    cy.get('html').click(400,135);
    cy.get('input[name="search"]').type("store");
    cy.get('input[name="mile"]').type(50);
    cy.contains('Store').click();
    cy.contains('Search').click();
    cy.wait(300);
    cy.get(".table tr:nth-child(5) td:nth-child(1) a").click();
    cy.wait(300);
    cy.get('a[class="pagination-link"]').contains("2").click();
    cy.wait(300);
    cy.get('button').contains('Next').should('be.disabled');
    cy.get('button').contains('Previous').should('not.be.disabled');
  })

})