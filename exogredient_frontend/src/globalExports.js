import Vue from 'vue';

export const ApiDomainName = "https://localhost:44392";

export const SortByDistance = 'distance';
export const SortByIngredientNum = 'ingredientNum';
export const SearchByIngredient = 'ingredient';
export const SearchByStore = 'store';

export const MinimumSearchTermLength = 1;
export const MinimumRadius = 1;
export const MaximumRadius = 100;

export const NoLastStoreData = -1;
export const NoLastStoreId = 0;
export const NoLastPageResultsNum = 0;
export const DefaultSkipPages = 1;
export const DefaultSortOption = 'distance';
export const DefaultFromSmallest = true;
export const DefaultPage = 1;
export const ResultsPerPage = 20;

export const Bus = new Vue();
export const AdminContact = "TEAMA.CS491@gmail.com"
export const ErrorHandler = function(router,response){
  if(400 <= response.status){
    if(router.currentRoute.path !== '/ErrorView'){
      router.push('/ErrorView');
      location.reload();
    }else{
      location.reload();
    }
  }
};