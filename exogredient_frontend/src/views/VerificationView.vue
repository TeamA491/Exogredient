<template>
    <div id="form">
        <div class='field'>
            <label class='label' for="phoneCode">Phone Code:</label><br>
            <input class="input" type="text" name="phoneCode" id="phoneCodeInput" placeholder="ex)1234" v-model="phoneCode">
            <a class='button' @click="sendPhoneCode">Resend</a><br>
            <span class='errorMessage' id="phoneCodeError"></span><br>
        </div>
        
        <div class="field">
            <label class="label" for="emailCode">Email Code:</label><br>
            <input class="input" type="text" name="emailCode" id="emailCodeInput" placeholder="ex)123456" v-model="emailCode">
            <a class='button' @click="sendEmailCode">Resend</a><br>
            <span class='errorMessage' id="emailCodeError"></span><br>
        </div>

        <input class="button" type="submit" value="Submit" :disabled="isSubmitDisabled" @click="submit">
    </div>
</template>

<script>
import * as global from "../globalExports.js";

export default {
    name: "VerificationView",
    data(){
        return{
            phoneCode:"",
            emailCode:"",

        }
    },
    computed:{
        isSubmitDisabled: function(){
            if(this.phoneCode.length === 0 || this.emailCode.length === 0){
                return true;
            }else{
                return false;
            }
        }
    },
    methods:{
        sendPhoneCode: async function(){
            fetch(`${global.ApiDomainName}/api/sendPhoneCode?`
            + `username=${this.$store.state.registration.username}&phoneNumber=${this.$store.state.registration.phoneNumber}`
            + `&ipAddress=${this.$store.state.userData.ipAddress}`);
        },
        sendEmailCode: async function(){
            fetch(`${global.ApiDomainName}/api/sendEmailCode?`
            + `username=${this.$store.state.registration.username}&email=${this.$store.state.registration.email}`
            + `&ipAddress=${this.$store.state.userData.ipAddress}`);
        },
        submit: async function(){
            var verifyEmailCodeResponse = await fetch(`${global.ApiDomainName}/api/verifyEmailCode?`
            +`username=${this.$store.state.registration.username}&emailCode=${this.emailCode}&`
            +`ipAddress=${this.$store.state.userData.ipAddress}`);

            global.ErrorHandler(this.$router, verifyEmailCodeResponse);

            var verifyPhoneCodeResponse = await fetch(`${global.ApiDomainName}/api/verifyPhoneCode?`
            +`username=${this.$store.state.registration.username}&phoneCode=${this.phoneCode}&`
            +`phoneNumber=${this.$store.state.registration.phoneNumber}&`
            +`ipAddress=${this.$store.state.userData.ipAddress}&`
            +`duringRegistration=${true}`);

            global.ErrorHandler(this.$router, verifyPhoneCodeResponse);

            var verifyEmailCodeJson = await verifyEmailCodeResponse.json();
            var verifyPhoneCodeJson = await verifyPhoneCodeResponse.json();

            if(!verifyEmailCodeJson.successful){
                document.getElementById('emailCodeError').innerText = verifyEmailCodeJson.message;
            }

            if(!verifyPhoneCodeJson.successful){
                document.getElementById('phoneCodeError').innerText = verifyPhoneCodeJson.message;
            }

            if(verifyEmailCodeJson.successful && verifyPhoneCodeJson.successful){
                this.$router.push('/login');
            }
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
    .errorMessage{
        font-size:11px;
        color: red;
        font-weight: bold;
    }
    .inputError{
        border-color: red;
    }
    .verifyError{
        font-size:13px;
        color: red;
        font-weight: bold;
    }
</style>