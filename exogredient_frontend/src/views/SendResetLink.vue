<template>
    <div id="form">
        <p><strong>Reset Password</strong></p>
        <div class='field'>
            <label class='label' for="username">Username:</label><br>
            <input class="input" type="text" name="username" id="usernameInput" placeholder="username" v-model="username" @blur="checkUsername"><br>
            <span class='errorMessage' id="usernameError"></span><br>
        </div>
        <span id="sendError"></span><br>
        <input class="button" type="submit" value="Send" :disabled="isSubmitDisabled" @click="submit">
    </div>
</template>

<script>
import * as global from "../globalExports.js";
export default {
    name:"SendResetLink",
    data(){
        return{
            username: "",
            fieldValidated: false
        }
    },
    computed:{
        isSubmitDisabled: function(){
            return !this.fieldValidated;
        }
    },
    methods:{
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
            this.fieldValidated = noError;
        },
        submit: async function(){
            var sendResponse = await fetch(`${global.ApiDomainName}/api/sendResetLink?username=${this.username}&`
            + `ipAddress=${this.$store.state.ipAddress}`);
            
            global.ErrorHandler(sendResponse, this.$router);

            var sendJson = await sendResponse.json();

            if(!sendJson){
                document.getElementById('sendError').innerText = "Username does not exist!";
            }
            alert("Email has been sent!");
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
    .sendError{
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