import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuex from 'vuex';
import createPersistedState from "vuex-persistedstate";
import App from './App.vue';
import './registerServiceWorker';
import SearchResultsView from './pages/SearchResultsView';
import StoreView from './pages/StoreView';
require("./assets/main.scss");

Vue.config.productionTip = false;
Vue.use(VueRouter);
Vue.use(Vuex);

const store = new Vuex.Store({
  plugins: [createPersistedState({
    storage: window.sessionStorage,
  })],
  state:{
    searchData:{},
    storeResults: [],
    ingredientResults: [],
    storeViewData: null,
    totalResultsNum:null,
    sortOption:{by:'distance', fromSmallest: true},
    username: "anonymous",
    ipAddress: "127.1.1.0",
  },
  mutations:{
    updateSearchData (state, newSearchData){
      state.searchData = newSearchData;
    },
    updateStoreResults (state, newStoreResults){
      state.storeResults = newStoreResults;
    },
    updateIngredientResults (state, newIngredientResults){
      state.ingredientResults = newIngredientResults;
    },
    updateStoreViewData (state, newStoreViewData){
      state.storeViewData = newStoreViewData;
    },
    updateTotalResultsNum(state, newTotalResultsNum){
      state.totalResultsNum = newTotalResultsNum;
    },
    updateSortOption(state, newSortOption){
      state.sortOption = newSortOption;
    }
  },
  actions:{
    updateSearchData ({commit}, newSearchData){
      commit('updateSearchData', newSearchData);
    },
    updateStoreResults ({commit}, newStoreResults){
      commit('updateStoreResults',newStoreResults)
    },
    updateIngredientResults ({commit}, newIngredientResults){
      commit('updateIngredientResults', newIngredientResults);
    },
    updateStoreViewData ({commit}, newStoreViewData){
      commit('updateStoreViewData', newStoreViewData);
    },
    updateTotalResultsNum({commit}, newTotalResultsNum){
      commit('updateTotalResultsNum', newTotalResultsNum);
    },
    updateSortOption({commit}, newSortOption){
      commit('updateSortOption', newSortOption);
    },
    sortStoreResults ({state}, sortOption){
      if(sortOption.by === 'distance'){
        if(sortOption.fromSmallest){
          state.storeResults.sort((a,b)=>a.distance-b.distance)
        }
        else{
          state.storeResults.sort((a,b)=>b.distance-a.distance)
        }
      }
      else if(sortOption.by === 'ingredientNum'){
        if(sortOption.fromSmallest){
          state.storeResults.sort((a,b)=>a.ingredientNum-b.ingredientNum)
        }
        else{
          state.storeResults.sort((a,b)=>b.ingredientNum-a.ingredientNum)
        }
      }
      
    }
  },
  getters:{
    storeResults: state=> {
      return state.storeResults;
    },

    ingredientResults: state=>{
      return state.ingredientResults;
    }
  }
});

const routes = [
  { path: '/SearchResultsView', component: SearchResultsView},
  { path: '/StoreView', component: StoreView}
]
const router = new VueRouter({
  routes
});

new Vue({
  render: function (h) { return h(App) },
  router,
  store
}).$mount('#app');
