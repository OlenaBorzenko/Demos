import { inject, observer } from 'mobx-react';
import React from 'react';

import { Section, PageContainer, Button } from '@shared/components';
import { useStyles } from './Home.style';
import { IHomeStore } from './store';
import { HOME_INJECTION_KEY } from '@shared/store/app';

interface Props {
}

interface InjectedProps extends Props {
  homeStore: IHomeStore;
}

export const HomeComponent: React.FC<Props> = observer(props => {
  const s = useStyles();

  const homeStore = (props as InjectedProps).homeStore;

  return (
    <>
      <PageContainer>
        <Section type="main" className={s.content}>
          I am so exited to be Your future application!
          <div>{homeStore.sampleText}</div>
          <div>{homeStore.capitilized}</div>
          <Button onClick={homeStore.invokeTestChange}>Click to trigger state change</Button>
        </Section>
      </PageContainer>
    </>
  );
});

export const Home = inject(HOME_INJECTION_KEY)(HomeComponent);
