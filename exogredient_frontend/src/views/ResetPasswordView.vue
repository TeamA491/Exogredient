<template>
    <div id="form">
        <div class="field">
            <label class="label" for="username">Username:</label><br>
            <input class="input" type="text" name="username" id="usernameInput" placeholder="Username" v-model="username" @blur="checkUsername"><br>
            <span class='errorMessage' id="usernameError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="password">New Password:</label><br>
            <input class="input" :type="passwordFieldType" name="password" id="passwordInput" placeholder="Password" v-model="password" @blur="checkPassword">
            <a class="button" @click='showHidePassword'>Show/Hide</a><br>
            <span class='errorMessage' id="passwordError"></span><br>
        </div>

        <div class="field">
            <label class="label" for="rePassword">Re-enter New password:</label><br>
            <input class="input" :type="rePasswordFieldType" name="rePassword" id="rePasswordInput" placeholder="Password" v-model="rePassword" @blur="checkRePassword">
            <a class="button" @click='showHideRePassword'>Show/Hide</a><br>
            <span class='errorMessage' id="rePasswordError"></span><br>
        </div>

        <span id='resetError'></span><br>
        <input class="button" type="submit" value="Reset" :disabled="isSubmitDisabled" @click="submit">
    </div>
</template>

<script>
import * as global from "../globalExports.js"
export default {
    name: "ResetPasswordView",
    props:["token"],
    data(){
        return{
            passwordFieldType: 'password',
            rePasswordFieldType: 'password',
            username: '',
            password:"",
            rePassword:"",
            fieldsValidation:{
                username: false,
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
        showHidePassword: function(){
            this.passwordFieldType = this.passwordFieldType === 'password' ? 'text':'password';
        },
        showHideRePassword: function(){
            this.rePasswordFieldType = this.rePasswordFieldType === 'password' ? 'text':'password';
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

            var resetResponse = await fetch(`${global.ApiDomainName}/api/updatePassword?username=${this.username}&`
                                            +`ipAddress=${this.$store.state.ipAddress}&hashedPassword=${hashedPassword}&`
                                            +`proxyPassword=${proxyPassword}&salt=${salt}&token=${this.token}`);

            global.ErrorHandler(resetResponse, this.$router);

            var resetJson = await resetResponse.json();

            if(!resetJson.successful){
                document.getElementById('resetError').innerText = resetJson.message;
                return;
            }

            this.$router.push('/login/reset');
        }
    }
}
</script>

<style scoped>
    input{
        border-style:solid;
    }
    #form{
        text-align: center;
        align-content: center;
    }
    #resetError{
        font-size:13px;
        color: red;
        font-weight: bold;
    }
    .inputError{
        border-color: red;
    }
    .errorMessage{
        font-size:11px;
        color: red;
        font-weight: bold;
    }
</style>