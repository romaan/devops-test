import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { Link, RouteComponentProps } from 'react-router-dom';
import { Button, Row, Col } from 'reactstrap';
import { ICrudGetAction, TextFormat } from 'react-jhipster';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { IRootState } from 'app/shared/reducers';
import { getEntity } from './release.reducer';
import { IRelease } from 'app/shared/model/release.model';
import { APP_DATE_FORMAT, APP_LOCAL_DATE_FORMAT } from 'app/config/constants';

export interface IReleaseDetailProps extends StateProps, DispatchProps, RouteComponentProps<{ id: string }> {}

export const ReleaseDetail = (props: IReleaseDetailProps) => {
  useEffect(() => {
    props.getEntity(props.match.params.id);
  }, []);

  const { releaseEntity } = props;
  return (
    <Row>
      <Col md="8">
        <h2>
          Release [<b>{releaseEntity.id}</b>]
        </h2>
        <dl className="jh-entity-details">
          <dt>
            <span id="version">Version</span>
          </dt>
          <dd>{releaseEntity.version}</dd>
          <dt>
            <span id="created">Created</span>
          </dt>
          <dd>
            {releaseEntity.created ? <TextFormat value={releaseEntity.created} type="date" format={APP_DATE_FORMAT} /> : null}
          </dd>
          <dt>Project</dt>
          <dd>{releaseEntity.projectId ? releaseEntity.projectId : ''}</dd>
          <dt>Retained</dt>
          <dd>{releaseEntity.deployments?.length > 0 ? [<span key='reason'>Yes, because of deployments: </span>, <span key='deployments'>{releaseEntity.deployments.map(item => {return item.id}).join(', ')}</span>]: 'No, will be deleted soon' } </dd>
        </dl>
        <Button tag={Link} to="/release" replace color="info">
          <FontAwesomeIcon icon="arrow-left" /> <span className="d-none d-md-inline">Back</span>
        </Button>
        &nbsp;
        <Button tag={Link} to={`/release/${releaseEntity.id}/edit`} replace color="primary">
          <FontAwesomeIcon icon="pencil-alt" /> <span className="d-none d-md-inline">Edit</span>
        </Button>
      </Col>
    </Row>
  );
};

const mapStateToProps = ({ release }: IRootState) => ({
    releaseEntity: release.entity,
});

const mapDispatchToProps = { getEntity };

type StateProps = ReturnType<typeof mapStateToProps>;
type DispatchProps = typeof mapDispatchToProps;

export default connect(mapStateToProps, mapDispatchToProps)(ReleaseDetail);
