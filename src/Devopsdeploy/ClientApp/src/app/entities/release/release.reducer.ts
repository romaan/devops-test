import axios from 'axios';
import { ICrudGetAction, ICrudGetAllAction, ICrudPutAction, ICrudDeleteAction } from 'react-jhipster';

import { cleanEntity } from 'app/shared/util/entity-utils';
import { REQUEST, SUCCESS, FAILURE } from 'app/shared/reducers/action-type.util';

import { IRelease, defaultValue } from 'app/shared/model/release.model';

export const ACTION_TYPES = {
  FETCH_RELEASE_LIST: 'release/FETCH_RELEASE_LIST',
  FETCH_RELEASE: 'release/FETCH_RELEASE',
  CREATE_RELEASE: 'release/CREATE_RELEASE',
  UPDATE_RELEASE: 'release/UPDATE_RELEASE',
  DELETE_RELEASE: 'release/DELETE_RELEASE',
  RESET: 'release/RESET',
};

const initialState = {
  loading: false,
  errorMessage: null,
  entities: [] as ReadonlyArray<IRelease>,
  entity: defaultValue,
  updating: false,
  totalItems: 0,
  updateSuccess: false,
};

export type ReleaseState = Readonly<typeof initialState>;

// Reducer

export default (state: ReleaseState = initialState, action): ReleaseState => {
  switch (action.type) {
    case REQUEST(ACTION_TYPES.FETCH_RELEASE_LIST):
    case REQUEST(ACTION_TYPES.FETCH_RELEASE):
      return {
        ...state,
        errorMessage: null,
        updateSuccess: false,
        loading: true,
      };
    case REQUEST(ACTION_TYPES.CREATE_RELEASE):
    case REQUEST(ACTION_TYPES.UPDATE_RELEASE):
    case REQUEST(ACTION_TYPES.DELETE_RELEASE):
      return {
        ...state,
        errorMessage: null,
        updateSuccess: false,
        updating: true,
      };
    case FAILURE(ACTION_TYPES.FETCH_RELEASE_LIST):
    case FAILURE(ACTION_TYPES.FETCH_RELEASE):
    case FAILURE(ACTION_TYPES.CREATE_RELEASE):
    case FAILURE(ACTION_TYPES.UPDATE_RELEASE):
    case FAILURE(ACTION_TYPES.DELETE_RELEASE):
      return {
        ...state,
        loading: false,
        updating: false,
        updateSuccess: false,
        errorMessage: action.payload,
      };
    case SUCCESS(ACTION_TYPES.FETCH_RELEASE_LIST):
      return {
        ...state,
        loading: false,
        entities: action.payload.data,
        totalItems: parseInt(action.payload.headers['x-total-count'], 10),
      };
    case SUCCESS(ACTION_TYPES.FETCH_RELEASE):
      return {
        ...state,
        loading: false,
        entity: action.payload.data,
      };
    case SUCCESS(ACTION_TYPES.CREATE_RELEASE):
    case SUCCESS(ACTION_TYPES.UPDATE_RELEASE):
      return {
        ...state,
        updating: false,
        updateSuccess: true,
        entity: action.payload.data,
      };
    case SUCCESS(ACTION_TYPES.DELETE_RELEASE):
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

const apiUrl = 'api/releases';

// Actions

export const getEntities: ICrudGetAllAction<IRelease> = (page, size, sort) => {
  const requestUrl = `${apiUrl}${sort ? `?page=${page}&size=${size}&sort=${sort}` : ''}`;
  return {
    type: ACTION_TYPES.FETCH_RELEASE_LIST,
    payload: axios.get<IRelease>(`${requestUrl}${sort ? '&' : '?'}cacheBuster=${new Date().getTime()}`),
  };
};

export const getEntity: ICrudGetAction<IRelease> = id => {
  const requestUrl = `${apiUrl}/${id}`;
  return {
    type: ACTION_TYPES.FETCH_RELEASE,
    payload: axios.get<IRelease>(requestUrl),
  };
};

export const createEntity: ICrudPutAction<IRelease> = entity => async dispatch => {
  const result = await dispatch({
    type: ACTION_TYPES.CREATE_RELEASE,
    payload: axios.post(apiUrl, cleanEntity(entity)),
  });
  dispatch(getEntities());
  return result;
};

export const updateEntity: ICrudPutAction<IRelease> = entity => async dispatch => {
  const result = await dispatch({
    type: ACTION_TYPES.UPDATE_RELEASE,
    payload: axios.put(apiUrl, cleanEntity(entity)),
  });
  return result;
};

export const deleteEntity: ICrudDeleteAction<IRelease> = id => async dispatch => {
  const requestUrl = `${apiUrl}/${id}`;
  const result = await dispatch({
    type: ACTION_TYPES.DELETE_RELEASE,
    payload: axios.delete(requestUrl),
  });
  return result;
};

export const reset = () => ({
  type: ACTION_TYPES.RESET,
});
