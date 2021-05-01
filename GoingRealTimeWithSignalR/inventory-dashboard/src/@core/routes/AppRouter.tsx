import { History } from 'history';
import React from 'react';
import { Route, Router, Switch, Redirect } from 'react-router-dom';

import { ErrorPage, getNotFoundPageConfig, Header } from '@shared/components';
import { Home } from 'Home/Home';

type Props = {
  history: History;
};

export const AppRouter: React.FC<Props> = ({ history }) => (
  <Router history={history}>
    <>
      <Header />
      <Switch>
        <Route
          exact={true}
          path="/"
          render={() => <Redirect to="/home" />}
        />
        <Route
          exact={true}
          path="/home"
          component={() => <Home/>}
        />
        <Route render={() => <ErrorPage config={getNotFoundPageConfig(history.goBack)} />}/>
      </Switch>
    </>
  </Router>
);
