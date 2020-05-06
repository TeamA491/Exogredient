import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from "vuex-persistedstate";

Vue.use(Vuex)

const store = new Vuex.Store({
  plugins: [createPersistedState({
    storage: window.sessionStorage,
  })],
  state:{
    inProgressUpload:{
      name: "",
      description: "",
      price: 0,
      priceUnit: "",
      rating: 0,
      image: null,
      id: -1
    },
    searchData:{},
    storeResults: [],
    ingredientResults: [],
    ingredientsList: [],
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
    userData:{
      username: "Anonymous",
      ipAddress: "",
      location: "",
      token: "",
      userType: "Anonymous"
    },
    routeChange:{
      to:"",
      from: ""
    }
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
      state.userData.username = newUsername;
    },
    updateIpAddress(state, newIpAddress){
      state.userData.ipAddress = newIpAddress;
    },
    updateLocation(state, newLocation){
      state.userData.location = newLocation;
    },
    updateRegistrationUsername(state, newUsername){
      state.registration.username = newUsername;
    },
    updateRegistrationPhoneNum(state, newPhoneNum){
      state.registration.phoneNumber = newPhoneNum;
    },
    updateIngredientsList(state, newIngredientsList) {
      state.ingredientsList = newIngredientsList;
    },
    updateToken(state, newToken){
      state.userData.token = newToken;
    },
    updateUserType(state, newUserType){
      state.userData.userType = newUserType;
    },
    updateEmail(state, newEmail){
      state.registration.email = newEmail;
    },
    updateRouteTo(state, newRouteTo){
      state.routeChange.to = newRouteTo;
    },
    updateRouteFrom(state, newRouteFrom){
      state.routeChange.from = newRouteFrom;
    },
    updateInProgressUploadName(state, newName){
      state.inProgressUpload.name = newName;
    },
    updateInProgressUploadDescription(state, newDescription){
      state.inProgressUpload.description = newDescription;
    },
    updateInProgressUploadPrice(state, newPrice){
      state.inProgressUpload.price = newPrice;
    },
    updateInProgressUploadPriceUnit(state, newPriceUnit){
      state.inProgressUpload.priceUnit = newPriceUnit;
    },
    updateInProgressUploadRating(state, newRating){
      state.inProgressUpload.rating = newRating;
    },
    updateInProgressUploadImage(state, newImage){
      state.inProgressUpload.image = newImage;
    },
    updateInProgressUploadId(state, newId){
      state.inProgressUpload.id = newId;
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
    updateInProgressUploadName({commit}, newName){
      commit('updateInProgressUploadName', newName);
    },
    updateInProgressUploadDescription({commit}, newDescription){
      commit('updateInProgressUploadDescription', newDescription);
    },
    updateInProgressUploadPrice({commit}, newPrice){
      commit('updateInProgressUploadPrice', newPrice);
    },
    updateInProgressUploadPriceUnit({commit}, newPriceUnit){
      commit('updateInProgressUploadPriceUnit', newPriceUnit);
    },
    updateInProgressUploadRating({commit}, newRating){
      commit('updateInProgressUploadRating', newRating);
    },
    updateInProgressUploadImage({commit}, newImage){
      commit('updateInProgressUploadImage', newImage);
    },
    updateInProgressUploadId({commit}, newId){
      commit('updateInProgressUploadId', newId);
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
    updateRouteTo({commit}, newRouteTo){
      commit('updateRouteTo', newRouteTo);
    },
    updateRouteFrom({commit}, newRouteFrom){
      commit('updateRouteFrom', newRouteFrom);
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
    },
    updateIngredientsList({ commit }, newIngredientsList) {
      commit("updateIngredientsList", newIngredientsList);
    },
  },
  getters:{
    storeResults: state=> {
      return state.storeResults;
    },
    inProgressUpload: state=> {
      return state.inProgressUpload;
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
