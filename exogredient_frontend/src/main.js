import Vue from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'
require("./assets/main.scss");

Vue.config.productionTip = false

new Vue({
  vuetify,
  router,
  store,
  render: function (h) { return h(App) }
}).$mount('#app')
