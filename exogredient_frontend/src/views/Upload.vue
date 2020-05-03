<template>
  <div>
    <form onsubmit="return false">
      <br />
      <div style="font-size: 150%; font-weight: bold; display: block; margin-left: auto; margin-right: auto; width:274px">UPLOAD AN INGREDIENT</div><br />
      <input type="file" id="fileInput" name="formFile" @change="AnalyzeImage" accept="image/*" style="display: block; margin-left: auto; margin-right: auto; width:202px;" required /> <br />

      <div id="loadingTitle" style="display: none; font-family: Gill Sans; font-size: 120%; font-weight: bold; margin-left: auto; margin-right: auto; width:154px;">Analyzing Image...</div>
      <img src="../assets/loader.gif" id="loading" style="display: none; margin-left: auto; margin-right: auto; width:150px; padding: 18px"></img>

      <div id="theRest" style="display:none">
        <!-- Ingredient Name -->
        <div style="font-family: Gill Sans; font-size: 100%; display: block; margin-left: auto; margin-right: auto; width:108px">Ingredient Name</div>
        <textarea v-on:keyup="IngredientNameKeyUp" maxlength="100" class="longInput" placeholder="Enter here..." cols="30" rows="3" id="name" style="max-height:75px; word-wrap: break-word; text-align: left; vertical-align:top; max-width: 600px; display: block; margin-left: auto; margin-right: auto; width: 55vh; height: 25vh; padding: 5px; border:1px solid #000000" required />
        <span id="chars1" style="font-family: Gill Sans; display: block; margin-left: auto; margin-right: auto; width:162px">100 characters remaining</span><br />

        <div style="text-align: center;">
          <div style="font-family: Gill Sans; font-size: 120%; display: inline-block; margin-left: auto; margin-right: auto; width:30px">$</div>
          <input type="text" id="price" placeholder="price..." style="max-width: 150px; display: inline-block; margin-left: auto; margin-right: auto; width: 20%; padding: 5px;border:1px solid #000000" required />
          <div style="font-family: Gill Sans; display: inline-block; margin-left: auto; margin-right: auto; width: 65px">per</div> 
          <select id="priceUnit" style="max-width: 150px; display: inline-block; margin-left: auto; margin-right: auto; width: 20%; padding: 5px;border:1px solid #000000" required>
            <option value="" disabled selected hidden>price unit...</option>
            <option value="item">Item</option>
            <option value="pound">Pound</option>
            <option value="gram">Gram</option>
            <option value="oz">OZ</option>
          </select><br /><br />
        </div>

        <div style="text-align: center;">
          <select id="rating" style="display: inline-block; margin-left: auto; margin-right: auto; width: 40px; padding: 5px;border:1px solid #000000" required>
              <option value="" disabled selected hidden>.....</option>
              <option value="1">1</option>
              <option value="2">2</option>
              <option value="3">3</option>
              <option value="4">4</option>
              <option value="4">5</option>
          </select>
          <div style="font-family: Gill Sans; font-size: 150%; display: inline-block; margin-left: auto; margin-right: auto; width:30px">★</div><br /><br />
        </div>


        <!-- Description -->
        <div style="font-family: Gill Sans;font-size: 100%; display: block; margin-left: auto; margin-right: auto; width:95px">Description</div>
        <textarea v-on:keyup="DescriptionKeyUp" class="longInput" maxlength="200" cols="30" rows="3" id="description" placeholder="Enter here..." style="max-height:125px; word-wrap: break-word; text-align: left; vertical-align:top; max-width: 600px; display: block; margin-left: auto; margin-right: auto; width: 70vh; height: 30vh; padding: 5px; border:1px solid #000000" required />
        <span id="chars2" style="font-family: Gill Sans;display: block; margin-left: auto; margin-right: auto; width:162px">200 characters remaining</span><br />


        <!-- Buttons -->
        <div style="text-align: center;">
          <input @click="SubmitUpload" type="submit" value="Submit" style="min-width: 100px; max-width: 600px; display: inline-block; margin-left: auto; margin-right: auto; width: 20%; padding: 5px;font-weight: bold; color: black; background-color: #1fe000; padding:8px; border:1px solid #000000;" />
          <div style="display: inline-block; margin-left: auto; margin-right: auto; width: 5px"></div> 
          <input @click="SaveDraft" type="submit" value="Save Draft" style="min-width: 100px; max-width: 600px; display: inline-block; margin-left: auto; margin-right: auto; width: 20%; padding: 5px;font-weight: bold; color: black; background-color: #b2b2b2; padding:8px; border:1px solid #000000;" formnovalidate /><br />
        </div>

        <div style="text-align: center;">
          <div style="font-family: Gill Sans; display: inline-block; margin-left: auto; margin-right: auto; width: 150px">All Fields Required *</div>
          <div style="display: inline-block; margin-left: auto; margin-right: auto; width: 15%; max-width:80px"></div>
          <div style="font-family: Gill Sans; display: inline-block; margin-left: auto; margin-right: auto; width: 151px">* Only Image Required</div>
        </div>
      </div>
      
      <div id="snackbar"></div>

    </form>
  </div>
</template>

<script>
import * as global from "../globalExports.js";
const exifr = require('exifr');

export default {
  data() {
    return {
      file: null,
      fileExtension: null,
      imageSize: null,
      category: null,
      name: null,
      suggestions: [],
      description: null,
      rating: null,
      priceUnit: null,
      price: null
    };
  },
  methods: {
    async AnalyzeImage(event) {
      var descriptionDOM = document.getElementById("description");
      descriptionDOM.value = "";

      var nameDOM = document.getElementById("name")
      nameDOM.value = "";

      var ratingDOM = document.getElementById("rating");
      ratingDOM.value = "";

      var priceUnitDOM = document.getElementById("priceUnit");
      priceUnitDOM.value = "";

      var priceDOM = document.getElementById("price");
      priceDOM.value = "";

      var chars1DOM = document.getElementById("chars1");
      chars1DOM.innerHTML = global.IngredientNameMaxChars + " characters remaining";

      var chars2DOM = document.getElementById("chars2");
      chars2DOM.innerHTML = global.DescriptionMaxChars + " characters remaining";;

      document.getElementById("loading").style.display = "block";
      document.getElementById("loadingTitle").style.display = "block";
      document.getElementById("theRest").style.display = "none";

      this.file = event.target.files[0];

      var splitArray = this.file.name.split(".")
      var ext = "";

      for (var i = 0; i < splitArray.length; i++) {
        ext = splitArray[i];
      }

      var validExt = false;

      for (var i = 0; i < global.ValidImageExtensions.length; i++) {
        if (ext.toUpperCase() === global.ValidImageExtensions[i].toUpperCase()) {
          validExt = true;
        }
      }

      if (!validExt) {
        var message = "Error: Invalid image extension (valid: ";

        for (var i = 0; i < global.ValidImageExtensions.length; i++) {
          message = message + ".";
          message = message + global.ValidImageExtensions[i].toUpperCase();

          if (i !== global.ValidImageExtensions.length - 1) {
            message = message + ",";
          }
          else {
            message = message + ")";
          }
        }

        this.MakeToast(message);

        var formFile = document.getElementById("fileInput");

        formFile.value = null;

        document.getElementById("loading").style.display = "none";
        document.getElementById("loadingTitle").style.display = "none";

        return;
      }

      if (this.file.size < global.MinimumPhotoSize || this.file.size > global.MaximumPhotoSize) {
        this.MakeToast("Error: Your image was not within requirements (" + global.MinimumPhotoSizeString + " to " + global.MaximumPhotoSizeString + ")");

        var formFile = document.getElementById("fileInput");

        formFile.value = null;

        document.getElementById("loading").style.display = "none";
        document.getElementById("loadingTitle").style.display = "none";

        return;
      }

      try {
        let output = await exifr.parse(this.file);
      }
      catch {
        this.MakeToast("Error: Your image did not have location metadata");

        var formFile = document.getElementById("fileInput");

        formFile.value = null;

        document.getElementById("loading").style.display = "none";
        document.getElementById("loadingTitle").style.display = "none";

        return;
      }

      this.fileExtension = "." + ext;
      this.imageSize = this.file.size;

      var formD = new FormData();
      formD.append(global.FileKey, this.file);
      formD.append(global.UsernameKey, "thesmokinggun42");
      formD.append(global.IPAddressKey, this.$store.state.ipAddress);

      fetch(`${global.ApiDomainName}/api/Upload/Vision`, {
        method: "POST",
        mode: "cors",
        body: formD
      })
      .then((response) => {
        if (response > 400) {
          document.getElementById("loading").style.display = "none";
          document.getElementById("loadingTitle").style.display = "none";
          
          this.MakeToast("Server error, please try again later.")
        }

        return response.json();
      })
      .then((data)=> {
        console.log(data);

        document.getElementById("loading").style.display = "none";
        document.getElementById("loadingTitle").style.display = "none";

        if (data[global.ExceptionOccurredResponseKey]) {
          this.MakeToast(data[global.MessageResponseKey]);
          var formFile = document.getElementById("fileInput");

          formFile.value = null;
        }
        else {
          var descriptionDOM = document.getElementById("description");
          var nameDOM = document.getElementById("name")
          var ratingDOM = document.getElementById("rating");
          var priceUnitDOM = document.getElementById("priceUnit");
          var priceDOM = document.getElementById("price");
          descriptionDOM.style.border = "1px solid #000000";
          nameDOM.style.border = "1px solid #000000";
          ratingDOM.style.border = "1px solid #000000";
          priceUnitDOM.style.border = "1px solid #000000";
          priceDOM.style.border = "1px solid #000000";

          document.getElementById("loading").style.display = "none";
          document.getElementById("loadingTitle").style.display = "none";
          document.getElementById("theRest").style.display = "block";

          this.category = data[global.CategoryResponseKey];
          this.name = data[global.NameResponseKey];
          this.suggestions = data[global.SuggestionsResponseKey];

          if (this.name === "") {
            this.MakeToast("Couldn't generate ingredient name, please enter it.")
          }
          else {
            document.getElementById("name").value = this.name;
          }
        }
      })
    },   
    SubmitUpload: function() {
      var descriptionDOM = document.getElementById("description");
      var description = descriptionDOM.value;

      var nameDOM = document.getElementById("name")
      var name = nameDOM.value;

      var ratingDOM = document.getElementById("rating");
      var rating = ratingDOM.options[ratingDOM.selectedIndex].value;

      var priceUnitDOM = document.getElementById("priceUnit");
      var priceUnit = priceUnitDOM.options[priceUnitDOM.selectedIndex].value;

      var priceDOM = document.getElementById("price");
      var price = priceDOM.value;

      descriptionDOM.style.border = "1px solid #000000";
      nameDOM.style.border = "1px solid #000000";
      ratingDOM.style.border = "1px solid #000000";
      priceUnitDOM.style.border = "1px solid #000000";
      priceDOM.style.border = "1px solid #000000";

      var validForm = true;

      if (description.length < global.DescriptionMinChars || description.length > global.DescriptionMaxChars) {
        validForm = false;
        descriptionDOM.style.border = "1px solid red";
      }

      if (name.length < global.IngredientNameMinChars || name.length > global.IngredientNameMaxChars) {
        validForm = false;
        nameDOM.style.border = "1px solid red";
      }

      if (rating < global.MinimumRating || rating > global.MaximumRating) {
        validForm = false;
        ratingDOM.style.border = "1px solid red";
      }

      if (price === "") {
        validForm = false;
        priceDOM.style.border = "1px solid red";
      }

      if (isNaN(price)) {
        validForm = false;
        priceDOM.style.border = "1px solid red";
        this.MakeToast("Price must be a number");
        
      }
      else {
        var priceValue = parseFloat(price);

        if (price < global.MinimumPrice || price > global.MaximumPrice) {
          validForm = false;
          priceDOM.style.border = "1px solid red";
          this.MakeToast("Price value invalid (must be between " + global.MinimumPrice + " and " + global.MaximumPrice + ")");
        }
      }

      var validPriceUnit = false;
      for (var i = 0; i < global.ValidPriceUnits.length; i++) {
        if (global.ValidPriceUnits[i] === priceUnit) {
          validPriceUnit = true;
        }
      }

      if (!validPriceUnit) {
        validForm = false;
        priceUnitDOM.style.border = "1px solid red";
      }

      if (!validForm) {
        return;
      }
      else {
        this.name = name;
        this.description = description;
        this.rating = parseInt(rating);
        this.priceUnit = priceUnit;
        this.price = parseFloat(price);
        
        var formD = new FormData();
        formD.append(global.FileKey, this.file);
        formD.append(global.UsernameKey, "thesmokinggun42");
        formD.append(global.IPAddressKey, this.$store.state.ipAddress);
        formD.append(global.CategoryKey, this.category);
        formD.append(global.NameKey, this.name);
        formD.append(global.DescriptionKey, this.description);
        formD.append(global.RatingKey, this.rating);
        formD.append(global.PriceKey, this.price);
        formD.append(global.PriceUnitKey, this.priceUnit);
        formD.append(global.ExtensionKey, this.fileExtension);
        formD.append(global.ImageSizeKey, this.imageSize);

        fetch(`${global.ApiDomainName}/api/Upload/NewUpload`, {
          method: "POST",
          mode: "cors",
          body: formD
        })
        .then((response) => {
          if (response > 400) {
            this.MakeToast("Server error, please try again later.")
          }

          return response.json();
        })
        .then((data)=> {
          console.log(data);

          if (data[global.ExceptionOccurredResponseKey] || !data[global.SuccessResponseKey]) {
            this.MakeToast(data[global.MessageResponseKey]);
          }
          else {
            // Go to profile view
          }
        })
      }
    },
    SaveDraft: function() {
      var descriptionDOM = document.getElementById("description");
      var description = descriptionDOM.value;

      var nameDOM = document.getElementById("name")
      var name = nameDOM.value;

      var ratingDOM = document.getElementById("rating");
      var rating = ratingDOM.options[ratingDOM.selectedIndex].value;

      var priceUnitDOM = document.getElementById("priceUnit");
      var priceUnit = priceUnitDOM.options[priceUnitDOM.selectedIndex].value;

      var priceDOM = document.getElementById("price");
      var price = priceDOM.value;

      var validForm = true;

      if (description !== "") {
        if (description.length < global.DescriptionMinChars || description.length > global.DescriptionMaxChars) {
          validForm = false;
        }

      }
      
      if (name !== "") {
        if (name.length < global.IngredientNameMinChars || name.length > global.IngredientNameMaxChars) {
          validForm = false;
        }
      }
      
      if (rating !== "") {
        if (rating < global.MinimumRating || rating > global.MaximumRating) {
          validForm = false;
        }
      }

      if (price !== "") {
        if (isNaN(price)) {
          validForm = false;
          this.MakeToast("Price must be a number");
          
        }
        else {
          var priceValue = parseFloat(price);

          if (price < global.MinimumPrice || price > global.MaximumPrice) {
            validForm = false;
            this.MakeToast("Price value invalid (must be between " + global.MinimumPrice + " and " + global.MaximumPrice + ")");
          }
        }
      }

      var validPriceUnit = false;
      if (priceUnit !== "") {
        for (var i = 0; i < global.ValidPriceUnits.length; i++) {
          if (global.ValidPriceUnits[i] === priceUnit) {
            validPriceUnit = true;
          }
        }

        if (!validPriceUnit) {
          validForm = false;
        }
      }

      if (!validForm) {
        return;
      }
      else {
        this.name = name;
        this.description = description;
        this.rating = parseInt(rating);
        this.priceUnit = priceUnit;
        this.price = parseFloat(price);
        
        var formD = new FormData();
        formD.append(global.FileKey, this.file);
        formD.append(global.UsernameKey, "thesmokinggun42");
        formD.append(global.IPAddressKey, this.$store.state.ipAddress);
        formD.append(global.CategoryKey, this.category);
        formD.append(global.NameKey, this.name);
        formD.append(global.DescriptionKey, this.description);
        formD.append(global.RatingKey, this.rating);
        formD.append(global.PriceKey, this.price);
        formD.append(global.PriceUnitKey, this.priceUnit);
        formD.append(global.ExtensionKey, this.fileExtension);
        formD.append(global.ImageSizeKey, this.imageSize);
        
        fetch(`${global.ApiDomainName}/api/Upload/DraftUpload`, {
          method: "POST",
          mode: "cors",
          body: formD
        })
        .then((response) => {
          if (response > 400) {
            this.MakeToast("Server error, please try again later.")
          }

          return response.json();
        })
        .then((data)=> {
          console.log(data);

          if (data[global.ExceptionOccurredResponseKey] || !data[global.SuccessResponseKey]) {
            this.MakeToast(data[global.MessageResponseKey]);
          }
          else {
            // Go to profile view
          }
        })
      }
    },
    MakeToast(data) {
      var snackbar = document.getElementById("snackbar");

      snackbar.className = "show";
      snackbar.innerHTML = data;

      // After 4 seconds, remove the show class from DIV
      setTimeout(function() { snackbar.className = snackbar.className.replace("show", ""); }, 4000);
    },
    DescriptionKeyUp: function() {
      var length = document.getElementById("description").value.length;
      var length = document.getElementById("description").maxLength - length;
      document.getElementById("chars2").innerHTML = length + " characters remaining";
    },
    IngredientNameKeyUp: function() {
      var length = document.getElementById("name").value.length;
      var length = document.getElementById("name").maxLength - length;
      document.getElementById("chars1").innerHTML = length + " characters remaining";
    }
  }
};
</script>


<style scoped>
#snackbar {
  display: block;
  visibility: hidden; /* Hidden by default. Visible on click */
  margin-left: auto; /* Divide value of min-width by 2 */
  margin-right: auto;
  width: 450px;
  background-color: #333; /* Black background color */
  color: #fff; /* White text color */
  text-align: center; /* Centered text */
  border-radius: 2px; /* Rounded borders */
  padding: 16px; /* Padding */
  z-index: 1; /* Add a z-index if needed */
}

/* Show the snackbar when clicking on a button (class added with JavaScript) */
#snackbar.show {
  visibility: visible; /* Show the snackbar */
  /* Add animation: Take 0.5 seconds to fade in and out the snackbar.
  However, delay the fade out process for 3.5 seconds */
  -webkit-animation: fadein 0.5s, fadeout 0.5s 3.5s;
  animation: fadein 0.5s, fadeout 0.5s 3.5s;
}

/* Animations to fade the snackbar in and out */
@-webkit-keyframes fadein {
  from {bottom: 0; opacity: 0;}
  to {bottom: 30px; opacity: 1;}
}

@keyframes fadein {
  from {bottom: 0; opacity: 0;}
  to {bottom: 30px; opacity: 1;}
}

@-webkit-keyframes fadeout {
  from {bottom: 30px; opacity: 1;}
  to {bottom: 0; opacity: 0;}
}

@keyframes fadeout {
  from {bottom: 30px; opacity: 1;}
  to {bottom: 0; opacity: 0;}
}
</style>
