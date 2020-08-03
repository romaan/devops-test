import React from 'react';
import { Switch } from 'react-router-dom';

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import ErrorBoundaryRoute from 'app/shared/error/error-boundary-route';

import Environment from './environment';
import Project from './project';
import Release from './release';
import Deployment from './deployment';
import Config from './config';
/* jhipster-needle-add-route-import - JHipster will add routes here */

const Routes = ({ match }) => (
  <div>
    <Switch>
      {/* prettier-ignore */}
      <ErrorBoundaryRoute path={`${match.url}environment`} component={Environment} />
      <ErrorBoundaryRoute path={`${match.url}project`} component={Project} />
      <ErrorBoundaryRoute path={`${match.url}release`} component={Release} />
      <ErrorBoundaryRoute path={`${match.url}deployment`} component={Deployment} />
      <ErrorBoundaryRoute path={`${match.url}config`} component={Config} />
      {/* jhipster-needle-add-route-path - JHipster will add routes here */}
    </Switch>
  </div>
);

export default Routes;
