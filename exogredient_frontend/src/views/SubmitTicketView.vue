<template>

  
  <div class="section">
    <div class="container">
      <!-- TOP LEVEL NAV BAR -->
      <nav class="level">
        <!-- Left side -->
        <div class="level-left">
          <div class="level-item">
            <a href="/">
              <h1 class="title">Exogredient</h1>
            </a>
          </div>
        </div>
      </nav>

      <h1 class="title text-center">Submit a new ticket</h1>
      <button @click="hello"> click me</button>
      <!-- SUBMIT TICKET FORM -->
      <div class="field is-horizontal">
        <div class="field-label is-normal">
          <label class="label">Category</label>
        </div>

        <!-- CATEGORY SELECT -->
        <div class="field-body">
          <div class="field">
            <div class="control">
              <div class="select is-fullwidth" id="category-dropdown-parent">
                <select id="category-dropdown">
                  <option>Select a category</option>
                  <option>Bug</option>
                  <option>Error</option>
                  <option>Suggestion</option>
                  <option>Other</option>
                </select>
              </div>
            </div>

            <p class="help is-danger" id="category-error-message"></p>
          </div>
        </div>
      </div>

      <div class="field is-horizontal">
        <div class="field-label is-normal">
          <label class="label">Explain</label>
        </div>

        <!-- TEXT AREA -->
        <div class="field-body">
          <div class="field">
            <div class="control">
              <textarea v-model="desc" class="textarea" placeholder="Explain your issue here" id="text-area"></textarea>
            </div>

            <p class="help is-danger" id="text-area-error-message"></p>
          </div>
        </div>
      </div>

      <div class="field is-horizontal">
        <div class="field-label">
          <!-- Left empty for spacing -->
        </div>

        <!-- SUBMIT BUTTON -->
        <div class="field-body">
          <div class="field">
            <div class="control">
              <button  @click ="submitTick" class="button is-primary" id="submit-button">Submit Ticket</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<!-- ================================================= -->

<script>
import Vue from "vue";

export default {
  data() {
    return {
      desc: "",
      category: ""
    }
  },
  methods: {
    hello() {
      fetch("https://localhost:44303/api/ticketsystem/GetTicket")
      .then((response) => {
        alert(response)
        return response.json()
      })
      .then((data) => {
        alert(data)
      })
    },
    submitTicket()
    {
      // fetch"(https://localhost:44303/api/ticketsystem/GetTicket", {
      //   methods:"POST",
      //   cors: ""

      //   body: JSON.stringify({
      //     ticketCategory: this.category,
      //     description : this.desc
      //   })
      // })
    }
  }
};

var submitButton;

var categoryDropdown;
var categoryDropdownParent;
var categoryErrorMessage;

var textArea;
var textAreaErrorMessage;

// On document ready...
document.addEventListener("DOMContentLoaded", function() {
  submitButton = document.getElementById("submit-button");
  categoryDropdown = document.getElementById("category-dropdown");
  categoryDropdownParent = document.getElementById("category-dropdown-parent");
  categoryErrorMessage = document.getElementById("category-error-message");

  textArea = document.getElementById("text-area");
  textAreaErrorMessage = document.getElementById("text-area-error-message");

  submitButton.addEventListener("click", onSubmitButtonClick);
  categoryDropdown.addEventListener("change", onCategoryDropdownChange);
  textArea.addEventListener("change", onTextAreaChange);
});

/*
******************
CLICK EVENTS
******************
*/
function onSubmitButtonClick() {
  // Call this function to validate category input
  onCategoryDropdownChange();

  // Call this function to validate text area input
  onTextAreaChange();
}

/*
******************
CHANGE EVENTS
******************
*/
function onCategoryDropdownChange() {
  if (categoryDropdown.selectedIndex == 0) {
    displayCategoryError("Please select a category!");
    disableSubmitButton();
    return;
  } else {
    hideCategoryError();
    enableSubmitButton();
  }
}

function onTextAreaChange() {
  let textCharcterLength = textArea.value.length;
  if (textCharcterLength < 10) {
    displayTextAreaError("Minimum of 10 characters required.");
    disableSubmitButton();
    return;
  } else if (textCharcterLength > 500) {
    displayTextAreaError(
      "Character length must not exceed 500. You are " +
        (500 - textCharcterLength) +
        " characters over."
    );
    disableSubmitButton();
    return;
  } else {
    enableSubmitButton();
    hideTextAreaError();
  }
}

/*
******************
OTHER
******************
*/
function displayCategoryError(text) {
  categoryDropdownParent.classlist.add("is-danger");
  categoryErrorMessage.classlist.remove("is-hidden");

  categoryErrorMessage.innerHtml = text;
}

function hideCategoryError() {
  categoryDropdownParent.classlist.remove("is-danger");
  categoryErrorMessage.classlist.add("is-hidden");
}

function displayTextAreaError(text) {
  textArea.classlist.add("is-danger");
  textAreaErrorMessage.classlist.remove("is-hidden");

  textAreaErrorMessage.innerHtml = text;
}

function hideTextAreaError() {
  textArea.classlist.remove("is-danger");
  textAreaErrorMessage.classlist.add("is-hidden");
}

function disableSubmitButton() {
  submitButton.setAttribute("disabled", "");
}

function enableSubmitButton() {
  submitButton.removeAttribute("disabled");
}
</script>

<!-- ================================================= -->

<style scoped>
</style>
