<template>
  <div class="section">
    <div class="container">
      <h1 class="title text-center">Submit a new ticket</h1>
      <!-- SUBMIT TICKET FORM -->
      <div class="field is-horizontal">
        <div class="field-label is-normal">
          <label class="label">Category</label>
        </div>

        <!-- CATEGORY SELECT -->
        <div class="field-body">
          <div class="field">
            <div class="control">
              <div
                class="select is-fullwidth"
                id="category-dropdown-parent"
                ref="categoryDropdownParent"
              >
                <select id="category-dropdown" ref="categoryDropdown">
                  <option>Select a category</option>
                  <option>Bug</option>
                  <option>Error</option>
                  <option>Suggestion</option>
                  <option>Other</option>
                </select>
              </div>
            </div>

            <p class="help is-danger" id="category-error-message" ref="categoryErrorMessage"></p>
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
              <textarea
                v-model="desc"
                class="textarea"
                placeholder="Explain your issue here"
                id="text-area"
                ref="textArea"
                @change="onTextAreaChange"
              ></textarea>
            </div>

            <p class="help is-danger" id="text-area-error-message" ref="textAreaErrorMessage"></p>
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
              <button
                disabled
                class="button is-primary"
                id="submit-button"
                ref="submitButton"
                @click="onSubmitButtonClick"
              >Submit Ticket</button>
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
  name: "tickets-view",
  data() {
    return {
      desc: "",
      category: ""
    };
  },
  methods: {
    onSubmitButtonClick: function() {
      // Call this function to validate category input
      this.onCategoryDropdownChange();

      // Call this function to validate text area input
      this.onTextAreaChange();

      this.$refs.submitButton.classList.add("is-loading");

      var data = {
        category: this.$refs.categoryDropdown.options[
          this.$refs.categoryDropdown.selectedIndex
        ].text,
        description: this.$refs.textArea.value
      };

      fetch(`${global.ApiDomainName}/api/ticket/submitTicket`, {
        method: "POST",
        body: JSON.stringify(data)
      }).then(res => {
        this.$router.push("/");
      });
    },
    onCategoryDropdownChange: function() {
      if (this.$refs.categoryDropdown.selectedIndex == 0) {
        this.displayCategoryError("Please select a category!");
        this.disableSubmitButton();
        return;
      } else {
        this.hideCategoryError();
        this.enableSubmitButton();
      }
    },
    onTextAreaChange: function() {
      let textCharcterLength = this.$refs.textArea.value.length;

      // Make sure we are below 10 characters
      if (textCharcterLength < 10) {
        this.displayTextAreaError("Minimum of 10 characters required.");
        this.disableSubmitButton();
        return;

        // Restrict to 500 chars max
      } else if (textCharcterLength > 500) {
        this.displayTextAreaError(
          "Character length must not exceed 500. You are " +
            (500 - textCharcterLength) +
            " characters over."
        );
        this.disableSubmitButton();
        return;
      } else {
        this.enableSubmitButton();
        this.hideTextAreaError();
      }
    },
    displayCategoryError: function(message) {
      this.$refs.categoryDropdownParent.classList.add("is-danger");
      this.$refs.categoryErrorMessage.classList.remove("is-hidden");

      this.$refs.categoryErrorMessage.innerHTML = message;
    },
    hideCategoryError: function() {
      this.$refs.categoryDropdownParent.classList.remove("is-danger");
      this.$refs.categoryErrorMessage.classList.add("is-hidden");
    },
    displayTextAreaError: function(message) {
      this.$refs.textArea.classList.add("is-danger");
      this.$refs.textAreaErrorMessage.classList.remove("is-hidden");

      this.$refs.textAreaErrorMessage.innerHTML = message;
    },
    hideTextAreaError: function() {
      this.$refs.textArea.classList.remove("is-danger");
      this.$refs.textAreaErrorMessage.classList.add("is-hidden");
    },
    disableSubmitButton: function() {
      this.$refs.submitButton.setAttribute("disabled", "");
    },
    enableSubmitButton: function() {
      this.$refs.submitButton.removeAttribute("disabled");
    }
  },
  mounted: function() {
    this.disableSubmitButton();
  }
};
</script>

<!-- ================================================= -->

<style scoped>
</style>
