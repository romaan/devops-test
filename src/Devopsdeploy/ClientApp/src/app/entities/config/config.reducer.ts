import axios from 'axios';
import { ICrudGetAction, ICrudGetAllAction, ICrudPutAction, ICrudDeleteAction } from 'react-jhipster';

import { cleanEntity } from 'app/shared/util/entity-utils';
import { REQUEST, SUCCESS, FAILURE } from 'app/shared/reducers/action-type.util';

import { IConfig, defaultValue } from 'app/shared/model/config.model';

export const ACTION_TYPES = {
  FETCH_ENVIRONMENT_LIST: 'config/FETCH_ENVIRONMENT_LIST',
  FETCH_ENVIRONMENT: 'config/FETCH_ENVIRONMENT',
  CREATE_ENVIRONMENT: 'config/CREATE_ENVIRONMENT',
  UPDATE_ENVIRONMENT: 'config/UPDATE_ENVIRONMENT',
  DELETE_ENVIRONMENT: 'config/DELETE_ENVIRONMENT',
  RESET: 'config/RESET',
};

const initialState = {
  loading: false,
  errorMessage: null,
  entities: [] as ReadonlyArray<IConfig>,
  entity: defaultValue,
  updating: false,
  totalItems: 0,
  updateSuccess: false,
};

export type ConfigState = Readonly<typeof initialState>;

// Reducer

export default (state: ConfigState = initialState, action): ConfigState => {
  switch (action.type) {
    case REQUEST(ACTION_TYPES.FETCH_ENVIRONMENT_LIST):
    case REQUEST(ACTION_TYPES.FETCH_ENVIRONMENT):
      return {
        ...state,
        errorMessage: null,
        updateSuccess: false,
        loading: true,
      };
    case REQUEST(ACTION_TYPES.CREATE_ENVIRONMENT):
    case REQUEST(ACTION_TYPES.UPDATE_ENVIRONMENT):
    case REQUEST(ACTION_TYPES.DELETE_ENVIRONMENT):
      return {
        ...state,
        errorMessage: null,
        updateSuccess: false,
        updating: true,
      };
    case FAILURE(ACTION_TYPES.FETCH_ENVIRONMENT_LIST):
    case FAILURE(ACTION_TYPES.FETCH_ENVIRONMENT):
    case FAILURE(ACTION_TYPES.CREATE_ENVIRONMENT):
    case FAILURE(ACTION_TYPES.UPDATE_ENVIRONMENT):
    case FAILURE(ACTION_TYPES.DELETE_ENVIRONMENT):
      return {
        ...state,
        loading: false,
        updating: false,
        updateSuccess: false,
        errorMessage: action.payload,
      };
    case SUCCESS(ACTION_TYPES.FETCH_ENVIRONMENT_LIST):
      return {
        ...state,
        loading: false,
        entities: action.payload.data,
        totalItems: parseInt(action.payload.headers['x-total-count'], 10),
      };
    case SUCCESS(ACTION_TYPES.FETCH_ENVIRONMENT):
      return {
        ...state,
        loading: false,
        entity: action.payload.data,
      };
    case SUCCESS(ACTION_TYPES.CREATE_ENVIRONMENT):
    case SUCCESS(ACTION_TYPES.UPDATE_ENVIRONMENT):
      return {
        ...state,
        updating: false,
        updateSuccess: true,
        entity: action.payload.data,
      };
    case SUCCESS(ACTION_TYPES.DELETE_ENVIRONMENT):
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

const apiUrl = 'api/configs';

// Actions

export const getEntities: ICrudGetAllAction<IConfig> = (page, size, sort) => {
  const requestUrl = `${apiUrl}${sort ? `?page=${page}&size=${size}&sort=${sort}` : ''}`;
  return {
    type: ACTION_TYPES.FETCH_ENVIRONMENT_LIST,
    payload: axios.get<IConfig>(`${requestUrl}${sort ? '&' : '?'}cacheBuster=${new Date().getTime()}`),
  };
};

export const getEntity: ICrudGetAction<IConfig> = id => {
  const requestUrl = `${apiUrl}/${id}`;
  return {
    type: ACTION_TYPES.FETCH_ENVIRONMENT,
    payload: axios.get<IConfig>(requestUrl),
  };
};

export const createEntity: ICrudPutAction<IConfig> = entity => async dispatch => {
  const result = await dispatch({
    type: ACTION_TYPES.CREATE_ENVIRONMENT,
    payload: axios.post(apiUrl, cleanEntity(entity)),
  });
  dispatch(getEntities());
  return result;
};

export const updateEntity: ICrudPutAction<IConfig> = entity => async dispatch => {
  const result = await dispatch({
    type: ACTION_TYPES.UPDATE_ENVIRONMENT,
    payload: axios.put(apiUrl, cleanEntity(entity)),
  });
  return result;
};

export const deleteEntity: ICrudDeleteAction<IConfig> = id => async dispatch => {
  const requestUrl = `${apiUrl}/${id}`;
  const result = await dispatch({
    type: ACTION_TYPES.DELETE_ENVIRONMENT,
    payload: axios.delete(requestUrl),
  });
  return result;
};

export const reset = () => ({
  type: ACTION_TYPES.RESET,
});
