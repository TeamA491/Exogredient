import Vue from 'vue';
import createPersistedState from 'vuex-persistedstate';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
    plugins: [
        createPersistedState({
            storage: window.sessionStorage,
        }),
    ],
    state: {
      storeResults: [],
      ingredientResults: [],
      ingredientsList: [],
      storeViewData: null,
      username: "username",
      ipAddress: "127.1.1.0",
      ingredientName: "",
      role: "business_owner"
    },
    mutations: {
        updateStoreResults(state, newStoreResults) {
            state.storeResults = newStoreResults;
        },
        updateIngredientResults(state, newIngredientResults) {
            state.ingredientResults = newIngredientResults;
        },
        updateIngredientName(state, newIngredientName) {
            state.ingredientName = newIngredientName;
        },
        updateStoreViewData(state, newStoreViewData) {
            state.storeViewData = newStoreViewData;
        },
        updateUsername(state, newUsername) {
            state.username = newUsername;
        },
        setTickets(tickets) {
            state.tickets = tickets;
        },
    },
    actions: {
        updateStoreResults({ commit }, newStoreResults) {
            commit('updateStoreResults', newStoreResults);
        },
        updateIngredientResults({ commit }, newIngredientResults) {
            commit('updateIngredientResults', newIngredientResults);
        },
        updateIngredientName({ commit }, newIngredientName) {
            commit('updateIngredientName', newIngredientName);
        },
        updateStoreViewData({ commit }, newStoreViewData) {
            commit('updateStoreViewData', newStoreViewData);
        },
        updateUsername({ commit }, newUsername) {
            commit('updateUsername', newUsername);
        },
        setTickets({ commit }, tickets) {
            commit('setTickets', tickets);
        },
    },
    getters: {
        storeResults: (state) => {
            return state.storeResults;
        },
        ingredientResults: (state) => {
            return state.ingredientResults;
        },
        username: (state) => {
            return state.username;
        },
        tickets: (state) => {
            return state.tickets;
        },
    },
});
