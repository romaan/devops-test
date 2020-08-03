import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Link, RouteComponentProps } from 'react-router-dom';
import { Button, Row, Col, Label } from 'reactstrap';
import { AvFeedback, AvForm, AvGroup, AvInput } from 'availity-reactstrap-validation';
import { ICrudGetAction, ICrudGetAllAction, ICrudPutAction } from 'react-jhipster';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IRootState } from 'app/shared/reducers';

import { IRelease } from 'app/shared/model/release.model';
import { getEntities as getReleases } from 'app/entities/release/release.reducer';
import { IEnvironment } from 'app/shared/model/environment.model';
import { getEntities as getEnvironments } from 'app/entities/environment/environment.reducer';
import { getEntity, updateEntity, createEntity, reset } from './deployment.reducer';
import { IDeployment } from 'app/shared/model/deployment.model';
import { convertDateTimeFromServer, convertDateTimeToServer, displayDefaultDateTime } from 'app/shared/util/date-utils';
import { mapIdList } from 'app/shared/util/entity-utils';

export interface IDeploymentUpdateProps extends StateProps, DispatchProps, RouteComponentProps<{ id: string }> {}

export const DeploymentUpdate = (props: IDeploymentUpdateProps) => {
  const [releaseId, setReleaseId] = useState('0');
  const [environmentId, setEnvironmentId] = useState('0');
  const [isNew, setIsNew] = useState(!props.match.params || !props.match.params.id);

  const { deploymentEntity, releases, environments, loading, updating } = props;

  const handleClose = () => {
    props.history.push('/deployment' + props.location.search);
  };

  useEffect(() => {
    if (isNew) {
      props.reset();
    } else {
      props.getEntity(props.match.params.id);
    }

    props.getReleases();
    props.getEnvironments();
  }, []);

  useEffect(() => {
    if (props.updateSuccess) {
      handleClose();
    }
  }, [props.updateSuccess]);

  const saveEntity = (event, errors, values) => {
    values.deployedAt = convertDateTimeToServer(values.deployedAt);

    if (errors.length === 0) {
      const entity = {
        ...deploymentEntity,
        ...values,
      };

      if (isNew) {
        props.createEntity(entity);
      } else {
        props.updateEntity(entity);
      }
    }
  };

  return (
    <div>
      <Row className="justify-content-center">
        <Col md="8">
          <h2 id="devopsdeployApp.deployment.home.createOrEditLabel">Create or edit a Deployment</h2>
        </Col>
      </Row>
      <Row className="justify-content-center">
        <Col md="8">
          {loading ? (
            <p>Loading...</p>
          ) : (
            <AvForm model={isNew ? {} : deploymentEntity} onSubmit={saveEntity}>
              <AvGroup>
                <Label for="deployment-id">ID</Label>
                <AvInput id="deployment-id" type="text" className="form-control" name="id" readOnly={!isNew} required />
              </AvGroup>
              <AvGroup>
                <Label id="deployedAtLabel" for="deployment-deployedAt">
                  Deployed At
                </Label>
                <AvInput
                  id="deployment-deployedAt"
                  type="datetime-local"
                  className="form-control"
                  name="deployedAt"
                  placeholder={'YYYY-MM-DD HH:mm'}
                  value={isNew ? displayDefaultDateTime() : convertDateTimeFromServer(props.deploymentEntity.deployedAt)}
                  validate={{
                    required: { value: true, errorMessage: 'This field is required.' },
                  }}
                />
              </AvGroup>
              <AvGroup>
                <Label for="deployment-release">Release</Label>
                <AvInput id="deployment-release" type="select" className="form-control" name="releaseId" required>
                  {releases
                    ? releases.map(otherEntity => (
                        <option value={otherEntity.id} key={otherEntity.id}>
                          {otherEntity.id}
                        </option>
                      ))
                    : null}
                </AvInput>
                <AvFeedback>This field is required.</AvFeedback>
              </AvGroup>
              <AvGroup>
                <Label for="deployment-environment">Environment</Label>
                <AvInput id="deployment-environment" type="select" className="form-control" name="environmentId" required>
                  {environments
                    ? environments.map(otherEntity => (
                        <option value={otherEntity.id} key={otherEntity.id}>
                          {otherEntity.id}
                        </option>
                      ))
                    : null}
                </AvInput>
                <AvFeedback>This field is required.</AvFeedback>
              </AvGroup>
              <Button tag={Link} id="cancel-save" to="/deployment" replace color="info">
                <FontAwesomeIcon icon="arrow-left" />
                &nbsp;
                <span className="d-none d-md-inline">Back</span>
              </Button>
              &nbsp;
              <Button color="primary" id="save-entity" type="submit" disabled={updating}>
                <FontAwesomeIcon icon="save" />
                &nbsp; Save
              </Button>
            </AvForm>
          )}
        </Col>
      </Row>
    </div>
  );
};

const mapStateToProps = (storeState: IRootState) => ({
  releases: storeState.release.entities,
  environments: storeState.environment.entities,
  deploymentEntity: storeState.deployment.entity,
  loading: storeState.deployment.loading,
  updating: storeState.deployment.updating,
  updateSuccess: storeState.deployment.updateSuccess,
});

const mapDispatchToProps = {
  getReleases,
  getEnvironments,
  getEntity,
  updateEntity,
  createEntity,
  reset,
};

type StateProps = ReturnType<typeof mapStateToProps>;
type DispatchProps = typeof mapDispatchToProps;

export default connect(mapStateToProps, mapDispatchToProps)(DeploymentUpdate);
