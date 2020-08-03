import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { Link, RouteComponentProps } from 'react-router-dom';
import { Button, Row, Col } from 'reactstrap';
import { ICrudGetAction, TextFormat } from 'react-jhipster';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { IRootState } from 'app/shared/reducers';
import { getEntity } from './deployment.reducer';
import { IDeployment } from 'app/shared/model/deployment.model';
import { APP_DATE_FORMAT, APP_LOCAL_DATE_FORMAT } from 'app/config/constants';

export interface IDeploymentDetailProps extends StateProps, DispatchProps, RouteComponentProps<{ id: string }> {}

export const DeploymentDetail = (props: IDeploymentDetailProps) => {
  useEffect(() => {
    props.getEntity(props.match.params.id);
  }, []);

  const { deploymentEntity } = props;
  return (
    <Row>
      <Col md="8">
        <h2>
          Deployment [<b>{deploymentEntity.id}</b>]
        </h2>
        <dl className="jh-entity-details">
          <dt>
            <span id="deployedAt">Deployed At</span>
          </dt>
          <dd>
            {deploymentEntity.deployedAt ? <TextFormat value={deploymentEntity.deployedAt} type="date" format={APP_DATE_FORMAT} /> : null}
          </dd>
          <dt>Release</dt>
          <dd>{deploymentEntity.releaseId ? deploymentEntity.releaseId : ''}</dd>
          <dt>Environment</dt>
          <dd>{deploymentEntity.environmentId ? deploymentEntity.environmentId : ''}</dd>
        </dl>
        <Button tag={Link} to="/deployment" replace color="info">
          <FontAwesomeIcon icon="arrow-left" /> <span className="d-none d-md-inline">Back</span>
        </Button>
        &nbsp;
        <Button tag={Link} to={`/deployment/${deploymentEntity.id}/edit`} replace color="primary">
          <FontAwesomeIcon icon="pencil-alt" /> <span className="d-none d-md-inline">Edit</span>
        </Button>
      </Col>
    </Row>
  );
};

const mapStateToProps = ({ deployment }: IRootState) => ({
  deploymentEntity: deployment.entity,
});

const mapDispatchToProps = { getEntity };

type StateProps = ReturnType<typeof mapStateToProps>;
type DispatchProps = typeof mapDispatchToProps;

export default connect(mapStateToProps, mapDispatchToProps)(DeploymentDetail);
