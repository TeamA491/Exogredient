<template>
  <div id="form">
    <p v-if="afterVerified" id="after">Verification Success!</p>
    <p v-if="afterPasswordReset" id="after">Password Reset Success!</p>
    <div class="field">
      <label class="label" for="username">Username:</label>
      <br/>
      <input class="input" type="text" name="username" id="usernameInput" placeholder="username" v-model="username"/>
      <br/>
    </div>

    <div class="field">
      <label class="label" for="password">Password:</label>
      <br/>
      <input class="input" type="password" name="password" id="passwordInput" placeholder="password" v-model="password"/>
      <br/>
    </div>
    <span id="loginError"></span>
    <br/>
    <input class="button" type="submit" value="Register" @click="goToRegistration" />
    <input class="button" type="submit" value="Login" :disabled="isSubmitDisabled" @click="submit" />
    <br/>
    <input class="button" type="submit" value="Reset Password" @click="goToSendResetLink" />
  </div>
</template>

<script>
import * as global from "../globalExports.js";
export default {
  name: "LoginView",
  data() {
    return {
      username: "",
      password: "",
      registered: false,
      reset: false
    };
  },
  computed: {
    isSubmitDisabled: function() {
      return this.username.length === 0 || this.password.length === 0;
    },
    afterVerified: function(){
        if(this.$store.state.routeChange.from === 'verify'){
            return true;
        }else{
            return false;
        }
    },
    afterPasswordReset: function(){
        if(this.$store.state.routeChange.from === 'resetPassword'){
            return true;
        }else{
            return false;
        }
    }
  },
  methods: {
    goToSendResetLink: function() {
      this.$router.push("/sendResetLink");
    },
    goToRegistration: function() {
      if (this.$store.state.userData.location === "California") {
        this.$router.push("/register");
      } else {
        alert("You must be in California to register!");
      }
    },
    byteArrayToHex: function(array) {
      var temp = [];
      for (var i = 0; i < array.length; i++) {
        temp.push(array[i].toString(16).padStart(2, "0"));
      }
      return temp.join("");
    },
    hexStringToByteArray: function(hex) {
      var byteArray = new Uint8Array(hex.length / 2);
      for (let i = 0; i < hex.length; i += 2) {
        byteArray[i / 2] = parseInt(hex.substring(i, i + 2), 16);
      }
      return byteArray;
    },
    getKeyMaterial: function(password) {
      let enc = new TextEncoder();
      return window.crypto.subtle.importKey(
        "raw",
        enc.encode(password),
        "PBKDF2",
        false,
        ["deriveBits", "deriveKey"]
      );
    },
    submit: async function() {
      var saltResponse = await fetch(
        `${global.ApiDomainName}/api/getSalt?username=${this.username}`
      );
      var saltJson = await saltResponse.json();

      if (!saltJson.successful) {
        document.getElementById("loginError").innerText = saltJson.data;
        return;
      }

      var saltArray = this.hexStringToByteArray(saltJson.data);
      var keyMaterial = await this.getKeyMaterial(this.password);

      let key = await window.crypto.subtle.deriveKey(
        {
          name: "PBKDF2",
          salt: saltArray,
          iterations: global.HashIteration,
          hash: global.HashAlgorithm
        },
        keyMaterial,
        { name: "AES-GCM", length: global.DigestByteLength * 8 },
        true,
        ["encrypt", "decrypt"]
      );

      let exportedKey = await window.crypto.subtle.exportKey("raw", key);
      var keyBuffer = new Uint8Array(exportedKey);
      var hashedPassword = this.byteArrayToHex(keyBuffer);

      var loginResponse = await fetch(
        `${global.ApiDomainName}/api/authenticate?` +
          `username=${this.username}&hashedPassword=${hashedPassword}` +
          `&ipAddress=${this.$store.state.userData.ipAddress}`
      );

      global.ErrorHandler(this.$router, loginResponse);

      var loginJson = await loginResponse.json();

      if (loginJson.successful) {
        this.$store.dispatch("updateUsername", this.username);
        this.$store.dispatch("updateToken", loginJson.token);
        this.$store.dispatch("updateUserType", loginJson.userType);
        this.$router.push("/");
      } else {
        document.getElementById("loginError").innerText = loginJson.message;
      }
    }
  }
};
</script>

<style scoped>
#loginError {
  font-size: 13px;
  color: red;
  font-weight: bold;
}
#after {
  text-align: center;
  font-weight: bold;
}
#form {
  text-align: center;
  align-content: center;
}
</style>