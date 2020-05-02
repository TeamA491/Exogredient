import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from "vuex-persistedstate";

Vue.use(Vuex)

const store = new Vuex.Store({
  plugins: [createPersistedState({
    storage: window.sessionStorage,
  })],
  state:{
    searchData:{},
    storeResults: [],
    ingredientResults: [],
    storeViewData: {},
    totalResultsNum:{
      storeResultsTotalNum: null,
      ingredientResultsTotalNum: null
    },
    sortOption:{by:'distance', fromSmallest: true},
    currentPages:{
      searchResultsView: 1,
      storeView: 1
    },
    registration:{
      username:"",
      phoneNumber: "",
      email: ""
    },
    username: "Anonymous",
    ipAddress: "",
    location: "",
    token: "",
    userType: "Anonymous"
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
    updateStoreResultsTotalNum(state, newTotalResultsNum){
      state.totalResultsNum.storeResultsTotalNum = newTotalResultsNum;
    },
    updateIngredientResultsTotalNum(state, newTotalResultsNum){
      state.totalResultsNum.ingredientResultsTotalNum = newTotalResultsNum;
    },
    updateSortOption(state, newSortOption){
      state.sortOption = newSortOption;
    },
    updateSearchResultsViewCurrentPage(state, newCurrentPage){
      state.currentPages.searchResultsView = newCurrentPage;
    },
    updateStoreViewCurrentPage(state, newCurrentPage){
      state.currentPages.storeView = newCurrentPage;
    },
    updateUsername(state, newUsername) {
      state.username = newUsername;
    },
    updateIpAddress(state, newIpAddress){
      state.ipAddress = newIpAddress;
    },
    updateLocation(state, newLocation){
      state.location = newLocation;
    },
    updateRegistrationUsername(state, newUsername){
      state.registration.username = newUsername;
    },
    updateRegistrationPhoneNum(state, newPhoneNum){
      state.registration.phoneNumber = newPhoneNum;
    },
    updateToken(state, newToken){
      state.token = newToken;
    },
    updateUserType(state, newUserType){
      state.userType = newUserType;
    },
    updateEmail(state, newEmail){
      state.registration.email = newEmail;
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
    updateStoreResultsTotalNum({commit}, newTotalResultsNum){
      commit('updateStoreResultsTotalNum', newTotalResultsNum);
    },
    updateIngredientResultsTotalNum({commit}, newTotalResultsNum){
      commit('updateIngredientResultsTotalNum', newTotalResultsNum);
    },
    updateSortOption({commit}, newSortOption){
      commit('updateSortOption', newSortOption);
    },
    updateSearchResultsViewCurrentPage({commit}, newCurrentPage){
      commit('updateSearchResultsViewCurrentPage', newCurrentPage);
    },
    updateStoreViewCurrentPage({commit}, newCurrentPage){
      commit('updateStoreViewCurrentPage', newCurrentPage);
    },
    updateIpAddress({commit}, newIpAddress){
      commit('updateIpAddress', newIpAddress);
    },
    updateLocation({commit}, newLocation){
      commit('updateLocation', newLocation);
    },
    updateRegistrationUsername({commit}, newUsername){
      commit('updateRegistrationUsername', newUsername);
    },
    updateRegistrationPhoneNum({commit}, newPhoneNum){
      commit('updateRegistrationPhoneNum', newPhoneNum);
    },
    updateToken({commit}, newToken){
      commit('updateToken', newToken);
    },
    updateUserType({commit}, newUserType){
      commit('updateUserType', newUserType);
    },
    updateEmail({commit}, newEmail){
      commit('updateEmail', newEmail);
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

    },
    updateUsername({commit}, newUsername) {
      commit("updateUsername", newUsername)
    }
  },
  getters:{
    storeResults: state=> {
      return state.storeResults;
    },

    ingredientResults: state=>{
      return state.ingredientResults;
    },
    username: state => {
      return state.username;
    },
  }
});

export default store;
