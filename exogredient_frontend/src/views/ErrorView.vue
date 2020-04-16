<template>
    <div class="message is-warning">
        <center><strong>Something went wrong... Please try again later.<br>
        If problem continues contact {{adminContact}} </strong></center>
    </div>
</template>

<script>
import * as global from "../globalExports.js";
export default {
    name: "ErrorView",
    data() {
        return {
            adminContact: global.AdminContact
        }
    },
}
</script>