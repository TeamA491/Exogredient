import Vue from 'vue';

export const ApiDomainName = "https://localhost:5001";

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

export const FirstNameMin = 1;
export const FirstNameMax = 200;
export const LastNameMin = 1;
export const LastNameMax = 200;
export const EmailMin = 1;
export const EmailMax = 200;
export const UsernameMin = 1;
export const UsernameMax = 200;
export const PhoneNumberLength = 10;
export const PasswordMin = 12;
export const PasswordMax = 2000;
export const MaxByte = 255;


export const HashIteration = 500000;
export const DigestByteLength = 32;
export const HashAlgorithm = 'SHA-256';

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

