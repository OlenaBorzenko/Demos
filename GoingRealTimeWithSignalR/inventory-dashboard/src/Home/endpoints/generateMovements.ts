import { ApiResponse, EndpointCallParams, IApiStore } from '@core/Api';

type Payload = {
};

export const generateMovements = async (
  api: IApiStore,
  params: EndpointCallParams<Payload>,
): Promise<ApiResponse> => {
  const url = '/api/createMovements';

  return await api.get(url, params);
};
