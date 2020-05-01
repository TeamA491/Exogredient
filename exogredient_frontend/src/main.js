import Vue from 'vue';
import App from './App.vue';
import './registerServiceWorker';
import router from './router';
import store from './store/index.js';
import vuetify from './plugins/vuetify';
require('./assets/main.scss');

Vue.config.productionTip = false;

new Vue({
    vuetify,
    router,
    store,
    render: h => h(App),
}).$mount('#app');
