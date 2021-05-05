import { MobXProviderContext } from 'mobx-react';
import React from 'react';

export const useStores = <T = Record<string, any>>():T =>  React.useContext<T>(MobXProviderContext);
