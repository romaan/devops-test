import React from 'react';
import { Switch } from 'react-router-dom';

import ErrorBoundaryRoute from 'app/shared/error/error-boundary-route';

import Deployment from './deployment';
import DeploymentDetail from './deployment-detail';
import DeploymentUpdate from './deployment-update';
import DeploymentDeleteDialog from './deployment-delete-dialog';

const Routes = ({ match }) => (
  <>
    <Switch>
      <ErrorBoundaryRoute exact path={`${match.url}/:id/delete`} component={DeploymentDeleteDialog} />
      <ErrorBoundaryRoute exact path={`${match.url}/new`} component={DeploymentUpdate} />
      <ErrorBoundaryRoute exact path={`${match.url}/:id/edit`} component={DeploymentUpdate} />
      <ErrorBoundaryRoute exact path={`${match.url}/:id`} component={DeploymentDetail} />
      <ErrorBoundaryRoute path={match.url} component={Deployment} />
    </Switch>
  </>
);

export default Routes;
