import store from "../../../src/store/index";
import * as global from '../../../src/globalExports.js';

describe("Get profile score positive", () => {
  it("Visit the profile view and verify score is positive", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "username");

    // Act
    cy.contains("Profile").click();

    // Assert
    cy.url().should("include", "/profile");
    cy.contains("h1", "4");
  });
});

describe("Get profile score negative", () => {
  it("Visit the profile view and verify score is negative", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "testuser");

    // Act
    cy.contains("Profile").click();

    // Assert
    cy.url().should("include", "/profile");
    cy.contains("h1", "-");
  });
});

describe("Get zero profile Score", () => {
  it("Visit the profile view and verify score is zero", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "zero");

    // Act
    cy.contains("Profile").click();

    // Assert
    cy.url().should("include", "/profile");
    cy.contains("h1", "0");
  });
});

describe("Attempt to get the profile score of an non existent user", () => {
  it("Make API call for nonexistent user", () => {
    cy.request({
      url: `${global.ApiDomainName}/api/userprofile/profilescore/nonexi`,
      failOnStatusCode: false,
    }).should((response) => {
      expect(response.status).to.eq(404);
    });
  });
});

describe("Get the nonempty savelist for a user", () => {
  it("retrieve a savelist for a user and verify its results", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "username");

    // Act
    cy.contains("Profile").click();
    cy.contains("Show SaveList").click();

    // Assert
    cy.contains("beef");
    cy.contains("crab meat");
    cy.contains("soy sauce");
  });
});

describe("Get a empty savelist for a user", () => {
  it("retrieve an empty save list for a user and verify that it is empty", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "testuser");

    // Act
    cy.contains("Profile").click();
    cy.contains("Show SaveList").click();

    // Assert
    cy.contains("X").should("not.exist");
  });
});

describe("attempt to get the savelist of an non existent user", () => {
  it("make request for a nonexistent user", () => {
    cy.request({
      url: `${global.ApiDomainName}/api/Userprofile/SaveList/nonexistent/0`,
      failOnStatusCode: false,
    }).should((response) => {
      expect(response.status).to.eq(404);
    });
  });
});


describe("Get the nonempty recent uploads for existing user", () => {
  it("retrieve a non empty recent recent upload list and verify that it is not empty", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "username");

    // Act
    cy.contains("Profile").click();
    cy.contains("Show Recent Uploads").click();

    // Assert
    cy.contains("chicken");
  });
});

describe("Get the empty recent uploads for existing user", () => {
  it("retrieve an empty recent recent upload list and verify that it is empty", () => {
    // Arrange
    cy.visit("http://localhost:8080/#/");
    store.dispatch("updateUsername", "testuser");

    // Act
    cy.contains("Profile").click();
    cy.contains("Show Recent Uploads").click();

    // Assert
    cy.contains("X").should("not.exist");
  });
});

describe("attempt to get recent upload of an non existent user", () => {
  it("retrieve recent upload on nonexistent and verify that we get a 404 error", () => {
    cy.request({
      url: `${global.ApiDomainName}/api/Userprofile/SaveList/nonexistent/0`,
      failOnStatusCode: false,
    }).should((response) => {
      expect(response.status).to.eq(404);
    });
  });
});



describe("delete a recent upload", () => {
  it("delete a recent upload and verify that it was deleted", () => {

    cy.request({
      url: `${global.ApiDomainName}/api/Userprofile/Upload/username/20`,
      failOnStatusCode: false,
      method: "DELETE"
    }).should((response) => {
      expect(response.status).to.eq(200);
    });

  });
});




describe("attempt to delete a nonexistent recent upload", () => {
  it("attempt to delete a recent upload for a nonexistent user and verify the 404 response", () => {
    
    cy.request({
      url: `${global.ApiDomainName}/api/Userprofile/Upload/nonexistent/1`,
      failOnStatusCode: false,
      method: "DELETE"
    }).should((response) => {
      expect(response.status).to.eq(404);
    });

  });
});


