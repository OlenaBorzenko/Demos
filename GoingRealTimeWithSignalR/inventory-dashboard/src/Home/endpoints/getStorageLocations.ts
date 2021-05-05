import { ApiResponse, EndpointCallParams, IApiStore } from '@core/Api';
import { IStorageLocationModel } from 'Home/store/StorageLocationModel';

type Payload = {
};

export const getStorageLocations = async (
  api: IApiStore,
  params: EndpointCallParams<Payload>,
): Promise<ApiResponse<IStorageLocationModel[]>> => {
  const url = '/api/getStorageLocations';

  return await api.get(url, params);
};
