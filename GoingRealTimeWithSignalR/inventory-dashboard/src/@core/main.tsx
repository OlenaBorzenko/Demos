import { createBrowserHistory, History } from 'history';
import { Provider } from 'mobx-react';
import React from 'react';
import ReactDOM from 'react-dom';

// import { ErrorPage, getBootstrapFailedPageConfig } from '@shared/components';
// import { reloadPage } from '@shared/helpers/reloadPage';
import { App } from './App';
import { initializeDependenciesAndCreateStores, Stores } from './createStores';

const rootHtmlElement = document.getElementById('root');

const renderApp = (stores: Stores, history: History) => (
  ReactDOM.render(
    <Provider {...stores}>
      <App history={history} />
    </Provider>,
    rootHtmlElement,
  )
);

// const renderAppBootstrapError = () => (
//   ReactDOM.render(
//     <ErrorPage config={getBootstrapFailedPageConfig(reloadPage)} />,
//     rootHtmlElement,
//   )
// );

(async () => {
  const history = createBrowserHistory({ basename: '/' });

  try {
    const initialStores = await initializeDependenciesAndCreateStores();

    renderApp(initialStores, history);
  } catch (e) {
    // eslint-disable-next-line no-console
    console.error(e);
    // renderAppBootstrapError();
  }
})();

if (module.hot) {
  module.hot.accept();
}
