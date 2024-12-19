  (() => {
    const maximumRetryCount = 30;
    const retryIntervalMilliseconds = 1000;
    const reconnectModal = document.getElementById('components-reconnect-modal');
    const loadingProgress = document.getElementById('loading-progress');
  
    const startReconnectionProcess = () => {
      reconnectModal.style.display = 'block';
  
      let isCanceled = false;
      let failed = false;
  
      (async () => {
        for (let i = 0; i < maximumRetryCount; i++) {
            loadingProgress.innerText = `(${i + 1} / ${maximumRetryCount})`;
  
          await new Promise(resolve => setTimeout(resolve, retryIntervalMilliseconds));
  
          if (isCanceled) {
            return;
          }
  
          try {
            const result = await Blazor.reconnect();
            if (!result) {
              // サーバー再接続できたが、トークンが変わったため拒否された。
              // リロードして再取得する
              location.reload();
              return;
            }
  
            // サーバーに再接続できた
            return;
          } catch {
            // エラーでサーバーに到達できなかった
            failed = true; 
          }
        }
  
        // 時間切れで再接続できなかったのでシステム担当に連絡する旨の表示を実施
        failed = true; 
        if (failed) {
          document.getElementById('loading-failed').style.display = 'block';
          document.getElementById('loading-caption').style.display = 'none';
          loadingProgress.style.display = 'none';
        }
      })();
  
      return {
        cancel: () => {
          isCanceled = true;
          reconnectModal.style.display = 'none';
        },
      };
    };
  
    let currentReconnectionProcess = null;
  
    Blazor.start({
      circuit: {
        reconnectionHandler: {
          onConnectionDown: () => currentReconnectionProcess ??= startReconnectionProcess(),
          onConnectionUp: () => {
            currentReconnectionProcess?.cancel();
            currentReconnectionProcess = null;
          }
        }
      }
    });
  })();