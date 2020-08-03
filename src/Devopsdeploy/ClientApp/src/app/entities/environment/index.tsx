import React from 'react';
import { Switch } from 'react-router-dom';

import ErrorBoundaryRoute from 'app/shared/error/error-boundary-route';

import Environment from './environment';
import EnvironmentDetail from './environment-detail';
import EnvironmentUpdate from './environment-update';
import EnvironmentDeleteDialog from './environment-delete-dialog';

const Routes = ({ match }) => (
  <>
    <Switch>
      <ErrorBoundaryRoute exact path={`${match.url}/:id/delete`} component={EnvironmentDeleteDialog} />
      <ErrorBoundaryRoute exact path={`${match.url}/new`} component={EnvironmentUpdate} />
      <ErrorBoundaryRoute exact path={`${match.url}/:id/edit`} component={EnvironmentUpdate} />
      <ErrorBoundaryRoute exact path={`${match.url}/:id`} component={EnvironmentDetail} />
      <ErrorBoundaryRoute path={match.url} component={Environment} />
    </Switch>
  </>
);

export default Routes;
