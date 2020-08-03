import React from 'react';
import { Switch } from 'react-router-dom';

import ErrorBoundaryRoute from 'app/shared/error/error-boundary-route';

import Config from './config';
import ConfigDetail from './config-detail';
import ConfigUpdate from './config-update';
import ConfigDeleteDialog from './config-delete-dialog';

const Routes = ({ match }) => (
  <>
    <Switch>
      <ErrorBoundaryRoute exact path={`${match.url}/:id/delete`} component={ConfigDeleteDialog} />
      <ErrorBoundaryRoute exact path={`${match.url}/new`} component={ConfigUpdate} />
      <ErrorBoundaryRoute exact path={`${match.url}/:id/edit`} component={ConfigUpdate} />
      <ErrorBoundaryRoute exact path={`${match.url}/:id`} component={ConfigDetail} />
      <ErrorBoundaryRoute path={match.url} component={Config} />
    </Switch>
  </>
);

export default Routes;
