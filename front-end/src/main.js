import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuex from 'vuex';
import createPersistedState from "vuex-persistedstate";
import App from './App.vue';
import './registerServiceWorker';
import StoresView from './pages/StoresView';
import IngredientView from './pages/IngredientsView';
require("./assets/main.scss");

Vue.config.productionTip = false;
Vue.use(VueRouter);
Vue.use(Vuex);

const store = new Vuex.Store({
  plugins: [createPersistedState({
    storage: window.sessionStorage,
  })],
  state:{
    storeResults: [],
    ingredientResults: [],
    storeViewData: null,
    username: "anonymous",
    ipAddress: "127.1.1.0",
    ingredientName: ""
  },
  mutations:{
    updateStoreResults (state, newStoreResults){
      state.storeResults = newStoreResults;
    },
    updateIngredientResults (state, newIngredientResults){
      state.ingredientResults = newIngredientResults;
    },
    updateIngredientName (state, newIngredientName){
      state.ingredientName = newIngredientName;
    },
    updateStoreViewData (state, newStoreViewData){
      state.storeViewData = newStoreViewData;
    }
  },
  actions:{
    updateStoreResults ({commit}, newStoreResults){
      commit('updateStoreResults',newStoreResults)
    },
    updateIngredientResults ({commit}, newIngredientResults){
      commit('updateIngredientResults', newIngredientResults);
    },
    updateIngredientName ({commit}, newIngredientName){
      commit('updateIngredientName', newIngredientName);
    },
    updateStoreViewData ({commit}, newStoreViewData){
      commit('updateStoreViewData', newStoreViewData);
    }
  },
  getters:{
    storeResults: state =>{
      return state.storeResults;
    },
    ingredientResults: state=>{
      return state.ingredientResults;
    }
  }
});

const routes = [
  { path: '/StoresView', component: StoresView},
  { path: '/IngredientsView', component: IngredientView}
]
const router = new VueRouter({
  routes
});

new Vue({
  render: function (h) { return h(App) },
  router,
  store
}).$mount('#app');
