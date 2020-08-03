import axios from 'axios';
import { ICrudGetAction, ICrudGetAllAction, ICrudPutAction, ICrudDeleteAction } from 'react-jhipster';

import { cleanEntity } from 'app/shared/util/entity-utils';
import { REQUEST, SUCCESS, FAILURE } from 'app/shared/reducers/action-type.util';

import { IDeployment, defaultValue } from 'app/shared/model/deployment.model';

export const ACTION_TYPES = {
  FETCH_DEPLOYMENT_LIST: 'deployment/FETCH_DEPLOYMENT_LIST',
  FETCH_DEPLOYMENT: 'deployment/FETCH_DEPLOYMENT',
  CREATE_DEPLOYMENT: 'deployment/CREATE_DEPLOYMENT',
  UPDATE_DEPLOYMENT: 'deployment/UPDATE_DEPLOYMENT',
  DELETE_DEPLOYMENT: 'deployment/DELETE_DEPLOYMENT',
  RESET: 'deployment/RESET',
};

const initialState = {
  loading: false,
  errorMessage: null,
  entities: [] as ReadonlyArray<IDeployment>,
  entity: defaultValue,
  updating: false,
  totalItems: 0,
  updateSuccess: false,
};

export type DeploymentState = Readonly<typeof initialState>;

// Reducer

export default (state: DeploymentState = initialState, action): DeploymentState => {
  switch (action.type) {
    case REQUEST(ACTION_TYPES.FETCH_DEPLOYMENT_LIST):
    case REQUEST(ACTION_TYPES.FETCH_DEPLOYMENT):
      return {
        ...state,
        errorMessage: null,
        updateSuccess: false,
        loading: true,
      };
    case REQUEST(ACTION_TYPES.CREATE_DEPLOYMENT):
    case REQUEST(ACTION_TYPES.UPDATE_DEPLOYMENT):
    case REQUEST(ACTION_TYPES.DELETE_DEPLOYMENT):
      return {
        ...state,
        errorMessage: null,
        updateSuccess: false,
        updating: true,
      };
    case FAILURE(ACTION_TYPES.FETCH_DEPLOYMENT_LIST):
    case FAILURE(ACTION_TYPES.FETCH_DEPLOYMENT):
    case FAILURE(ACTION_TYPES.CREATE_DEPLOYMENT):
    case FAILURE(ACTION_TYPES.UPDATE_DEPLOYMENT):
    case FAILURE(ACTION_TYPES.DELETE_DEPLOYMENT):
      return {
        ...state,
        loading: false,
        updating: false,
        updateSuccess: false,
        errorMessage: action.payload,
      };
    case SUCCESS(ACTION_TYPES.FETCH_DEPLOYMENT_LIST):
      return {
        ...state,
        loading: false,
        entities: action.payload.data,
        totalItems: parseInt(action.payload.headers['x-total-count'], 10),
      };
    case SUCCESS(ACTION_TYPES.FETCH_DEPLOYMENT):
      return {
        ...state,
        loading: false,
        entity: action.payload.data,
      };
    case SUCCESS(ACTION_TYPES.CREATE_DEPLOYMENT):
    case SUCCESS(ACTION_TYPES.UPDATE_DEPLOYMENT):
      return {
        ...state,
        updating: false,
        updateSuccess: true,
        entity: action.payload.data,
      };
    case SUCCESS(ACTION_TYPES.DELETE_DEPLOYMENT):
      return {
        ...state,
        updating: false,
        updateSuccess: true,
        entity: {},
      };
    case ACTION_TYPES.RESET:
      return {
        ...initialState,
      };
    default:
      return state;
  }
};

const apiUrl = 'api/deployments';

// Actions

export const getEntities: ICrudGetAllAction<IDeployment> = (page, size, sort) => {
  const requestUrl = `${apiUrl}${sort ? `?page=${page}&size=${size}&sort=${sort}` : ''}`;
  return {
    type: ACTION_TYPES.FETCH_DEPLOYMENT_LIST,
    payload: axios.get<IDeployment>(`${requestUrl}${sort ? '&' : '?'}cacheBuster=${new Date().getTime()}`),
  };
};

export const getEntity: ICrudGetAction<IDeployment> = id => {
  const requestUrl = `${apiUrl}/${id}`;
  return {
    type: ACTION_TYPES.FETCH_DEPLOYMENT,
    payload: axios.get<IDeployment>(requestUrl),
  };
};

export const createEntity: ICrudPutAction<IDeployment> = entity => async dispatch => {
  const result = await dispatch({
    type: ACTION_TYPES.CREATE_DEPLOYMENT,
    payload: axios.post(apiUrl, cleanEntity(entity)),
  });
  dispatch(getEntities());
  return result;
};

export const updateEntity: ICrudPutAction<IDeployment> = entity => async dispatch => {
  const result = await dispatch({
    type: ACTION_TYPES.UPDATE_DEPLOYMENT,
    payload: axios.put(apiUrl, cleanEntity(entity)),
  });
  return result;
};

export const deleteEntity: ICrudDeleteAction<IDeployment> = id => async dispatch => {
  const requestUrl = `${apiUrl}/${id}`;
  const result = await dispatch({
    type: ACTION_TYPES.DELETE_DEPLOYMENT,
    payload: axios.delete(requestUrl),
  });
  return result;
};

export const reset = () => ({
  type: ACTION_TYPES.RESET,
});
