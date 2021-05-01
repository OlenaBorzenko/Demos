import { ApiResponse, EndpointCallParams, IApiStore } from '@core/Api';

type Payload = {
  someId: string;
};

type SomeData = {
  id: string;
  name: string;
};

export const endpointSample = async (
  api: IApiStore,
  params: EndpointCallParams<Payload>,
): Promise<ApiResponse<SomeData>> => {
  const url = '/api/someData';

  return await api.get(url, params);
};
