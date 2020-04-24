<template>
    <form>
        <div class='field'>
            <label class='label' for="fname">First Name:</label><br>
            <input class="input" type="text" name="fname" id="fnameInput" placeholder="First Name" v-model="firstName" @blur='checkFirstName'><br>
            <span class='errorMessage' id="fnameError"></span><br>
        </div>
        
        <div class="field">
            <label class="label" for="lname">Last Name:</label><br>
            <input class="input" type="text" name="lname" id="lnameInput" placeholder="Last Name" v-model="lastName" @blur="checkLastName"><br>
            <span class='errorMessage' id="lnameError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="username">Username:</label><br>
            <input class="input" type="text" name="username" id="usernameInput" placeholder="Username" v-model="username" @blur="checkUsername"><br>
            <span class='errorMessage' id="usernameError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="email">Email:</label><br>
            <input class="input" type="text" name="email" id="emailInput" placeholder="Email" v-model="email" @blur="checkEmail"><br>
            <span class='errorMessage' id="emailError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="phone">Phone Number:</label><br>
            <input class="input" type="text" name="phone" id="phoneInput" placeholder="Phone #" v-model="phoneNumber" @blur="checkPhoneNumber"><br>
            <span class='errorMessage' id="phoneError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="password">Password:</label><br>
            <input class="input" :type="passwordFieldType" name="password" id="passwordInput" placeholder="Password" v-model="password" @blur="checkPassword">
            <a class="button" @click='showHidePassword'>Show/Hide</a><br>
            <span class='errorMessage' id="passwordError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="rePassword">Re-enter Password:</label><br>
            <input class="input" :type="rePasswordFieldType" name="rePassword" id="rePasswordInput" placeholder="Password" v-model="rePassword" @blur="checkRePassword">
            <a class="button" @click='showHideRePassword'>Show/Hide</a><br>
            <span class='errorMessage' id="rePasswordError"></span><br>
        </div>

        <input class="button" type="submit" value="Submit" :disabled="isSubmitDisabled">
    </form>
</template>

<script>
export default {
    name: "RegistrationView",

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
            errors: null,
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
            if (this.firstName.length < 1) {
                document.getElementById('fnameError').innerText += 'First name required.\n';
                document.getElementById('fnameInput').classList.add('inputError');
                noError = false;
            }else if(this.firstName.length > 200){
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
            if (this.lastName.length < 1) {
                document.getElementById('lnameError').innerText += 'Last name required.\n';
                document.getElementById('lnameInput').classList.add('inputError');
                noError = false;
            }else if(this.lastName.length > 200){
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
            if (this.email.length < 1) {
                document.getElementById('emailError').innerText += 'Email required.\n';
                document.getElementById('emailInput').classList.add('inputError');
                noError = false;
            }else if(this.email.length > 200){
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
            if (this.username.length < 1) {
                document.getElementById('usernameError').innerText += 'Username required.\n';
                document.getElementById('usernameInput').classList.add('inputError');
                noError = false;
            }else if(this.username.length > 200){
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
            if (this.phoneNumber.length !== 10) {
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
            if (this.password.length < 12) {
                document.getElementById('passwordError').innerText += 'Password must be at least 12 characters.\n';
                document.getElementById('passwordInput').classList.add('inputError');
                noError = false;
            } else if (this.password.length > 2000) {
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
        checkForm: function (e) {
            this.errors = [];

            // Check first name length
            if (this.firstName.length < 1) {
                this.errors.push("First Name required.");
            }else if(this.firstName.length > 200){
                this.errors.push("First Name must be max 200 characters");
            }

            // Check first name characters
            if (!/^[\x00-\x7F]*$/.test(this.firstName) || /[<,>]/.test(this.firstName)) {
                this.errors.push("First Name must be alphanumeric" 
                + "or special characters (Except < and >)");
            }

            // Check last name length
            if (this.lastName.length < 1) {
                this.errors.push("Last Name required.");
            }else if(this.lastName.length > 200){
                this.errors.push("Last Name must be max 200 characters");
            }

            // Check last name characters
            if (!/^[\x00-\x7F]*$/.test(this.lastName) || /[<,>]/.test(this.lastName)) {
                this.errors.push("Last Name must be alphanumeric" 
                + "or special characters (Except < and >)");
            }

            // Check Email length
            if (this.email.length < 1) {
                this.errors.push('Email required.');
            } else if (this.email.length > 200) {
                this.errors.push('Email must be max length 200 characters');
            }

            // Check Email characters
            if (!/^[\x00-\x7F]*$/.test(this.lastName) || /[<,>]/.test(this.lastName)) {
                this.errors.push("Email must be alphanumeric" 
                + "or special characters (Except < and >)");
            }

            // Check Email format
            if(!validateEmailFormat(this.email)){
                this.errors.push("Email format is wrong");
            }

            // Check username length
            if (this.username.length < 1) {
                this.errors.push('Username required.');
            } else if (this.username.length > 200) {
                this.errors.push('Username must be max length 200 characters');
            }

            // Check username characters
            if (!/^[\x00-\x7F]*$/.test(this.username) || /[<,>]/.test(this.username)) {
                this.errors.push("Username must be alphanumeric" 
                + "or special characters (Except < and >)");
            }

            // Check phone number length
            if (this.phoneNumber.length !== 10) {
                this.errors.push('Phone number must be 10 digits.');
            }

            // Check phone number characters
            if (/[^0-9]/.test(this.phoneNumber)){
                this.errors.push('Phone number must be only digits');
            }

            // Check password length
            if (this.password.length < 12) {
                this.errors.push('Username required.');
            } else if (this.password.length > 2000) {
                this.errors.push('Username must be max length 200 characters');
            }

            // Check password characters
            if (!/^[\x00-\x7F]*$/.test(this.password)) {
                this.errors.push("Password must be alphanumeric" 
                + "or special characters");
            }

            // Check Re-entered password
            if(this.password !== this.rePassword){
                this.errors.push('Re-entered password does not match the password.')
            }
            e.preventDefault();
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
    .inputError{
        border-color: red;
    }
    input{
        border-style:solid;
    }
    form{
        text-align: center;
        align-content: center;
    }
</style>