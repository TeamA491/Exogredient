import Vue from "vue";
import VueRouter from "vue-router";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";
import App from "./App.vue";
import "./registerServiceWorker";
import StoresView from "./pages/StoresView";
import IngredientView from "./pages/IngredientsView";
import vuetify from './plugins/vuetify'

import store from "./store"
require("./assets/main.scss");

Vue.config.productionTip = false;
Vue.use(VueRouter);


const routes = [
  { path: "/StoresView", component: StoresView },
  { path: "/IngredientsView", component: IngredientView },
  {
    path: "/profile",
    name: "profile",
    component: () => import("@/pages/UserProfile.vue"),
  },
  {
    path: "/upload",
    name: "upload",
    component: () => import("@/pages/Upload.vue"),
  },
  {
    path: "/",
    name: "home",
  },
];
const router = new VueRouter({
  routes,
});

new Vue({
  render: function(h) {
    return h(App);
  },
  vuetify,
  router,
  store,
}).$mount("#app");

