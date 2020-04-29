<template>
    <div id="form">
        <p v-if="afterRegistered === 'true'" id="afterRegistration">Verification Success!</p>
        <div class='field'>
            <label class='label' for="username">Username:</label><br>
            <input class="input" type="text" name="username" id="usernameInput" placeholder="username" v-model="username"><br>
        </div>
        
        <div class="field">
            <label class="label" for="password">Password:</label><br>
            <input class="input" type="password" name="password" id="passwordInput" placeholder="password" v-model="password"><br>
        </div>
        <span id="loginError"></span><br>
        <input class="button" type="submit" value="Submit" :disabled="isSubmitDisabled" @click="submit">
    </div>
</template>

<script>
import * as global from "../globalExports.js";
export default {
    name: "LoginView",
    props:["afterRegistered"],
    data(){
        return{
            username: "",
            password: ""
        }
    },
    computed: {
        isSubmitDisabled: function(){
            return (this.username.length === 0 || this.password.length === 0);
        }
    },
    methods:{
        byteArrayToHex: function(array){
            var temp = [];
            for(var i=0; i<array.length; i++){
                temp.push(array[i].toString(16).padStart(2,'0'));
            }
            return temp.join("");
        },
        hexStringToByteArray: function(hex){
            var byteArray = new Uint8Array(hex.length/2);
            for(let i=0; i < hex.length; i+=2){
                byteArray[i/2] = parseInt(hex.substring(i,i+2), 16);
            }
            return byteArray;
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
        submit: async function(){
            var saltResponse = 
                await fetch(`${global.ApiDomainName}/api/login/getSalt?username=${this.username}`);
            var saltJson = await saltResponse.json();

            if(!saltJson.successful){
                document.getElementById('loginError').innerText = saltJson.data;
                return;
            }
            
            var saltArray = this.hexStringToByteArray(saltJson.data);
            var keyMaterial = await this.getKeyMaterial(this.password);

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

            var loginResponse = await fetch(`${global.ApiDomainName}/api/login/authenticate?`
                + `username=${this.username}&hashedPassword=${hashedPassword}`
                + `&ipAddress=${this.$store.state.ipAddress}`);
            
            global.ErrorHandler(this.$router,loginResponse);

            var loginJson = await loginResponse.json();

            if(loginJson.successful){
                this.$store.dispatch('updateUsername', this.username);
                this.$store.dispatch('updateToken', loginJson.token);
                this.$store.dispatch('updateUserType', loginJson.userType);
                this.$router.push('/');
            }else{
                document.getElementById('loginError').innerText = loginJson.message;
            }
        }
    }
}
</script>

<style scoped>
    #loginError{
        font-size:13px;
        color: red;
        font-weight: bold;
    }
    #afterRegistration{
        text-align: center;
        font-weight: bold;
    }
    #form{
        text-align: center;
        align-content: center;
    }
</style>