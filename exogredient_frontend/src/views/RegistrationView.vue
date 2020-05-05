<template>
    <div id="form">
        <span class='registerError'></span>
        <div class='field'>
            <label class='label' for="fname">First Name:</label><br>
            <input class="input" type="text" name="fname" id="fnameInput" placeholder="First Name" v-model="firstName" @input='checkFirstName'><br>
            <span class='errorMessage' id="fnameError"></span><br>
        </div>
        
        <div class="field">
            <label class="label" for="lname">Last Name:</label><br>
            <input class="input" type="text" name="lname" id="lnameInput" placeholder="Last Name" v-model="lastName" @input="checkLastName"><br>
            <span class='errorMessage' id="lnameError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="username">Username:</label><br>
            <input class="input" type="text" name="username" id="usernameInput" placeholder="Username" v-model="username" @input="checkUsername"><br>
            <span class='errorMessage' id="usernameError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="email">Email:</label><br>
            <input class="input" type="text" name="email" id="emailInput" placeholder="Email" v-model="email" @input="checkEmail"><br>
            <span class='errorMessage' id="emailError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="phone">Phone Number:</label><br>
            <input class="input" type="text" name="phone" id="phoneInput" placeholder="Phone #" v-model="phoneNumber" @input="checkPhoneNumber"><br>
            <span class='errorMessage' id="phoneError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="password">Password:</label><br>
            <input class="input" :type="passwordFieldType" name="password" id="passwordInput" placeholder="Password" v-model="password" @input="checkPassword">
            <a class="button" @click='showHidePassword'>Show/Hide</a><br>
            <span class='errorMessage' id="passwordError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="rePassword">Re-enter Password:</label><br>
            <input class="input" :type="rePasswordFieldType" name="rePassword" id="rePasswordInput" placeholder="Password" v-model="rePassword" @input="checkRePassword">
            <a class="button" @click='showHideRePassword'>Show/Hide</a><br>
            <span class='errorMessage' id="rePasswordError"></span><br>
        </div>

        <span class='registerError'></span><br>
        <input class="button" type="submit" value="Submit" :disabled="isSubmitDisabled" @click="submit">
    </div>
</template>

<script>
import * as global from "../globalExports.js";

export default {
    name: "RegistrationView",
    mounted(){
        if(this.$store.state.userData.location !== "California"){
            alert("You must be in California to register!");
            this.$router.push('/');
        }
    },
    data(){
        return{
            firstName: '',
            lastName: '',
            username: '',
            email: '',
            phoneNumber: '',
            password: '',
            rePassword: '',
            passwordFieldType: 'password',
            rePasswordFieldType: 'password',
            fieldsValidation:{
                firstName: false,
                lastName: false,
                username: false,
                email: false,
                phoneNumber: false,
                password: false,
                rePassword: false
            }
        }
    },

    computed:{
        isSubmitDisabled: function(){
            var noError = true;
            for(var property in this.fieldsValidation){
                noError = noError && this.fieldsValidation[property];
            }
            return !noError;
        }
    },
    methods:{
        showHidePassword: function(){
            this.passwordFieldType = this.passwordFieldType === 'password' ? 'text':'password';
        },
        showHideRePassword: function(){
            this.rePasswordFieldType = this.rePasswordFieldType === 'password' ? 'text':'password';
        },
        validateEmailFormat: function(email){
            var splitEmail = email.split("@");
            if(splitEmail.length === 2){
                var first = splitEmail[0];
                var second = splitEmail[1];

                if(first.length >= 1 && second.length >= 1){
                    if(first.includes('..') || second.includes('..')){
                        return false;
                    }else{
                        return true;
                    }
                }
            }
            return false;
        },
        checkFirstName: function(){
            document.getElementById('fnameError').innerText = '';
            document.getElementById('fnameInput').classList.remove('inputError');
            var noError = true;
            // Check first name length
            if (this.firstName.length < global.FirstNameMin) {
                document.getElementById('fnameError').innerText += 'First name required.\n';
                document.getElementById('fnameInput').classList.add('inputError');
                noError = false;
            }else if(this.firstName.length > global.FirstNameMax){
                document.getElementById('fnameError').innerText += "First Name must be max 200 characters.\n";
                document.getElementById('fnameInput').classList.add('inputError');
                noError = false;
            }

            // Check first name characters
            if (!/^[\x00-\x7F]*$/.test(this.firstName) || /[<,>]/.test(this.firstName)) {
                document.getElementById('fnameError').innerText += 'First Name must be alphanumeric' 
                + 'or special characters (Except < and >)\n';
                document.getElementById('fnameInput').classList.add('inputError');
                noError = false;
            }
            this.fieldsValidation.firstName = noError;
        },
        checkLastName: function(){
            document.getElementById('lnameError').innerText = '';
            document.getElementById('lnameInput').classList.remove('inputError');
            var noError = true;
            // Check last name length
            if (this.lastName.length < global.LastNameMin) {
                document.getElementById('lnameError').innerText += 'Last name required.\n';
                document.getElementById('lnameInput').classList.add('inputError');
                noError = false;
            }else if(this.lastName.length > global.LastNameMax){
                document.getElementById('lnameError').innerText += "Last Name must be max 200 characters.\n";
                document.getElementById('lnameInput').classList.add('inputError');
                noError = false;
            }

            // Check last name characters
            if (!/^[\x00-\x7F]*$/.test(this.lastName) || /[<,>]/.test(this.lastName)) {
                document.getElementById('lnameError').innerText += 'Last Name must be alphanumeric' 
                + 'or special characters (Except < and >)\n';
                document.getElementById('lnameInput').classList.add('inputError');
                noError = false;
            }
            this.fieldsValidation.lastName = noError;
        },
        checkEmail: function(){
            document.getElementById('emailError').innerText = '';
            document.getElementById('emailInput').classList.remove('inputError');
            var noError = true;
            // Check email length
            if (this.email.length < global.EmailMin) {
                document.getElementById('emailError').innerText += 'Email required.\n';
                document.getElementById('emailInput').classList.add('inputError');
                noError = false;
            }else if(this.email.length > global.EmailMax){
                document.getElementById('emailError').innerText += "Email must be max 200 characters.\n";
                document.getElementById('emailInput').classList.add('inputError');
                noError = false;
            }

            // Check email characters
            if (!/^[\x00-\x7F]*$/.test(this.email) || /[<,>]/.test(this.email)) {
                document.getElementById('emailError').innerText += 'Email must be alphanumeric' 
                + 'or special characters (Except < and >)\n';
                document.getElementById('emailInput').classList.add('inputError');
                noError = false;
            }

            // Check Email format
            if(!this.validateEmailFormat(this.email)){
                document.getElementById('emailError').innerText += "Email format is wrong\n";
                document.getElementById('emailInput').classList.add('inputError');
                noError = false;
            }

            this.fieldsValidation.email = noError;
        },
        checkUsername: function(){
            document.getElementById('usernameError').innerText = '';
            document.getElementById('usernameInput').classList.remove('inputError');
            var noError = true;

            // Check username length
            if (this.username.length < global.UsernameMin) {
                document.getElementById('usernameError').innerText += 'Username required.\n';
                document.getElementById('usernameInput').classList.add('inputError');
                noError = false;
            }else if(this.username.length > global.UsernameMax){
                document.getElementById('usernameError').innerText += "Username must be max 200 characters.\n";
                document.getElementById('usernameInput').classList.add('inputError');
                noError = false;
            }

            // Check username characters
            if (!/^[\x00-\x7F]*$/.test(this.username) || /[<,>]/.test(this.username)) {
                document.getElementById('usernameError').innerText += 'Username must be alphanumeric' 
                + 'or special characters (Except < and >)\n';
                document.getElementById('usernameInput').classList.add('inputError');
                noError = false;
            }
            this.fieldsValidation.username = noError;
        },
        checkPhoneNumber: function(){
            document.getElementById('phoneError').innerText = '';
            document.getElementById('phoneInput').classList.remove('inputError');
            var noError = true;

            // Check phone number length
            if (this.phoneNumber.length !== global.PhoneNumberLength) {
                document.getElementById('phoneError').innerText += 'Phone number must be 10 digits.\n';
                document.getElementById('phoneInput').classList.add('inputError');
                noError = false;
            }

            // Check phone number characters
            if (/[^0-9]/.test(this.phoneNumber)){
                document.getElementById('phoneError').innerText += 'Phone number must be only digits.\n'
                document.getElementById('phoneInput').classList.add('inputError');
                noError = false;
            }

            this.fieldsValidation.phoneNumber = noError;
        },
        checkPassword: function(){
            document.getElementById('passwordError').innerText = '';
            document.getElementById('rePasswordError').innerText = '';
            document.getElementById('passwordInput').classList.remove('inputError');
            document.getElementById('rePasswordInput').classList.remove('inputError');
            var noError = true;

            // Check password length
            if (this.password.length < global.PasswordMin) {
                document.getElementById('passwordError').innerText += 'Password must be at least 12 characters.\n';
                document.getElementById('passwordInput').classList.add('inputError');
                noError = false;
            } else if (this.password.length > global.PasswordMax) {
                document.getElementById('passwordError').innerText += 'Username can\'t be longer than 2000 characters\n';
                document.getElementById('passwordInput').classList.add('inputError');
                noError = false;
            }

            // Check password characters
            if (!/^[\x00-\x7F]*$/.test(this.password)) {
                document.getElementById('passwordError').innerText +="Password must be alphanumeric" 
                + "or special characters.\n";
                document.getElementById('passwordInput').classList.add('inputError');
                noError = false;
            }

            this.fieldsValidation.password = noError;
        },
        checkRePassword: function(){
            document.getElementById('rePasswordError').innerText = '';
            document.getElementById('rePasswordInput').classList.remove('inputError');
            var noError = true;

            // Check Re-entered password
            if(this.password !== this.rePassword){
                document.getElementById('rePasswordError').innerText += 
                'Re-entered password does not match the password.\n'
                document.getElementById('rePasswordInput').classList.add('inputError');
                noError = false;
            }

            this.fieldsValidation.rePassword = noError;
        },
        generateSalt: function(len){
            var saltArray = new Uint8Array(len);
            for(var i=0; i<len; i++){
                var random = Math.floor(Math.random() * global.MaxByte) + 1;
                saltArray[i] = random;
            }
            return saltArray;
        },
        getKeyMaterial: function(password){
            let enc = new TextEncoder();
            return window.crypto.subtle.importKey(
                "raw",
                enc.encode(password),
                "PBKDF2",
                false,
                ["deriveBits", "deriveKey"]
            );
        },
        byteArrayToHex: function(array){
            var temp = [];
            for(var i=0; i<array.length; i++){
                temp.push(array[i].toString(16).padStart(2,'0'));
            }
            return temp.join("");
        },
        submit: async function(){
            
            let saltArray = this.generateSalt(8);
            let keyMaterial = await this.getKeyMaterial(this.password);

            let key = await window.crypto.subtle.deriveKey(
                    {
                        "name": "PBKDF2",
                        salt: saltArray,
                        "iterations": global.HashIteration,
                        "hash": global.HashAlgorithm
                    },
                    keyMaterial,
                    { "name": "AES-GCM", "length": global.DigestByteLength*8},
                    true,
                    [ "encrypt", "decrypt" ]
                );

            let exportedKey = await window.crypto.subtle.exportKey("raw",key);
            var keyBuffer = new Uint8Array(exportedKey);
            var hashedPassword = this.byteArrayToHex(keyBuffer);
            var salt = this.byteArrayToHex(saltArray);
            var proxyPassword = "0".repeat(this.password.length);
            
            var registrationResponse = await fetch(`${global.ApiDomainName}/api/register?`
                +`firstName=${this.firstName}&lastName=${this.lastName}&`
                +`email=${this.email}&username=${this.username}&`
                +`phoneNumber=${this.phoneNumber}&ipAddress=${this.$store.state.userData.ipAddress}&`
                +`hashedPassword=${hashedPassword}&salt=${salt}&proxyPassword=${proxyPassword}`);

            global.ErrorHandler(this.$router,registrationResponse);

            var registrationJson = await registrationResponse.json();

            if(!registrationJson.successful){
                var elements = document.getElementsByClassName("registerError");
                for(let i=0; i<elements.length; i++){
                    elements[i].innerText = registrationJson.message;
                }
                return;
            }

            this.$store.dispatch('updateRegistrationUsername',this.username);
            this.$store.dispatch('updateRegistrationPhoneNum', this.phoneNumber);
            this.$store.dispatch('updateEmail', this.email);

            fetch(`${global.ApiDomainName}/api/sendEmailCode?`
            + `username=${this.username}&email=${this.email}`
            + `&ipAddress=${this.$store.state.userData.ipAddress}`);

            fetch(`${global.ApiDomainName}/api/sendPhoneCode?`
            + `username=${this.username}&phoneNumber=${this.phoneNumber}`
            + `&ipAddress=${this.$store.state.userData.ipAddress}`);

            this.$router.push('/verify');
        }
    }

    
}
</script>

<style scoped>
    .errorMessage{
        font-size:11px;
        color: red;
        font-weight: bold;
    }
    .registerError{
        font-size:13px;
        color: red;
        font-weight: bold;
    }
    .inputError{
        border-color: red;
    }
    input{
        border-style:solid;
    }
    #form{
        text-align: center;
        align-content: center;
    }
</style>