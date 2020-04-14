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

// Vue.use(Vuex);

// const store = new Vuex.Store({
//   plugins: [
//     createPersistedState({
//       storage: window.sessionStorage,
//     }),
//   ],
//   state: {
//     storeResults: [],
//     ingredientResults: [],
//     storeViewData: null,
//     username: "username",
//     ipAddress: "127.1.1.0",
//     ingredientName: "",
//     role: "business_owner"
//   },
//   mutations: {
//     updateStoreResults(state, newStoreResults) {
//       state.storeResults = newStoreResults;
//     },
//     updateIngredientResults(state, newIngredientResults) {
//       state.ingredientResults = newIngredientResults;
//     },
//     updateIngredientName(state, newIngredientName) {
//       state.ingredientName = newIngredientName;
//     },
//     updateStoreViewData(state, newStoreViewData) {
//       state.storeViewData = newStoreViewData;
//     },
//     updateUsername(state, newUsername) {
//       state.username = newUsername;
//     }
//   },
//   actions: {
//     updateStoreResults({ commit }, newStoreResults) {
//       commit("updateStoreResults", newStoreResults);
//     },
//     updateIngredientResults({ commit }, newIngredientResults) {
//       commit("updateIngredientResults", newIngredientResults);
//     },
//     updateIngredientName({ commit }, newIngredientName) {
//       commit("updateIngredientName", newIngredientName);
//     },
//     updateStoreViewData({ commit }, newStoreViewData) {
//       commit("updateStoreViewData", newStoreViewData);
//     },
//     updateUsername({commit}, newUsername) {
//       commit("updateUsername", newUsername)
//     }
//   },
//   getters: {
//     storeResults: state => {
//       return state.storeResults;
//     },
//     ingredientResults: state => {
//       return state.ingredientResults;
//     },
//     username: state => {
//       return state.username;
//     }
//   },
// });

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

